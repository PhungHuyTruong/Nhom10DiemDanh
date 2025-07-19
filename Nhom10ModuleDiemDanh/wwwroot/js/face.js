async function loadModels() {
    const MODEL_URL = '/models';
    await Promise.all([
        faceapi.nets.tinyFaceDetector.loadFromUri(MODEL_URL),
        faceapi.nets.faceRecognitionNet.loadFromUri(MODEL_URL),
        faceapi.nets.faceLandmark68Net.loadFromUri(MODEL_URL)
    ]);
    console.log('✅ Models loaded');
}

// === ATTENDANCE ===
async function initAttendance(apiUrl, callback) {
    console.log('🟢 Hàm initAttendance đã được gọi với URL:', apiUrl);

    const video = document.getElementById('videoInput');
    const statusDiv = document.getElementById('status');
    const studentId = document.getElementById('studentId')?.value;

    console.log('🟢 studentId:', studentId);

    await loadModels();

    try {
        const stream = await navigator.mediaDevices.getUserMedia({ video: {} });
        video.srcObject = stream;
    } catch (err) {
        console.error('❌ Lỗi khi truy cập camera:', err);
        statusDiv.innerText = 'Không thể truy cập camera';
        if (callback) callback(false);
        return;
    }

    // Đợi video load dữ liệu xong (tránh lỗi canvas)
    await new Promise(resolve => {
        if (video.readyState >= 3) {
            resolve();
        } else {
            video.addEventListener('loadeddata', () => resolve(), { once: true });
        }
    });

    // Xóa canvas cũ nếu có
    const oldCanvas = document.querySelector('canvas');
    if (oldCanvas) oldCanvas.remove();

    // Tạo canvas sau khi video đã load
    const canvas = faceapi.createCanvasFromMedia(video);
    document.body.appendChild(canvas);
    const displaySize = { width: video.width, height: video.height };
    faceapi.matchDimensions(canvas, displaySize);

    video.addEventListener('play', async () => {
        await new Promise(r => setTimeout(r, 3000)); // đợi camera ổn định

        const detection = await faceapi
            .detectSingleFace(video, new faceapi.TinyFaceDetectorOptions({ inputSize: 320, scoreThreshold: 0.4 }))
            .withFaceLandmarks();

        if (!detection) {
            statusDiv.innerText = '❌ Không phát hiện khuôn mặt nào!';
            if (callback) callback(false);
            return;
        }

        const resizedDetections = faceapi.resizeResults(detection, displaySize);
        canvas.getContext('2d').clearRect(0, 0, canvas.width, canvas.height);
        faceapi.draw.drawDetections(canvas, resizedDetections);

        // Chụp ảnh và gửi API
        const faceCanvas = document.createElement('canvas');
        faceCanvas.width = video.videoWidth;
        faceCanvas.height = video.videoHeight;
        faceCanvas.getContext('2d').drawImage(video, 0, 0, faceCanvas.width, faceCanvas.height);
        const imageBlob = await new Promise(resolve => faceCanvas.toBlob(resolve, 'image/jpeg'));

        const formData = new FormData();
        formData.append('image', imageBlob, 'capture.jpg');
        if (studentId) formData.append('studentId', studentId);

        try {
            const response = await fetch(apiUrl, {
                method: 'POST',
                body: formData
            });

            const result = await response.json();

            if (!result.success) {
                alert("❌ Không nhận diện được khuôn mặt phù hợp.");
                if (callback) callback(false);
                return;
            }

            alert(result.message);
            if (callback) callback(true);
        } catch (err) {
            console.error('❌ Lỗi gửi request:', err);
            alert('Lỗi gửi dữ liệu điểm danh.');
            if (callback) callback(false);
        }
    }, { once: true });

    // Bắt đầu chạy video
    video.play();
}


$('#faceModal').on('shown.bs.modal', function () {
    initAttendance('/Face/RecognizeFace');
});

// === REGISTRATION ===
window.addEventListener('DOMContentLoaded', () => {
    const video = document.getElementById('video');
    const canvas = document.getElementById('canvas');
    const statusDiv = document.getElementById('status');
    const instruction = document.getElementById('instruction');
    const idSinhVien = document.getElementById('video')?.dataset?.sinhvien || ''; // assume passed via data-sinhvien

    let capturedCount = 0;
    const maxCaptures = 5;
    const saveUrl = '/Face/SaveFace';
    const directions = [
        'Hãy nhìn THẲNG vào camera',
        'Quay mặt sang TRÁI',
        'Quay mặt sang PHẢI',
        'Nhìn LÊN một chút',
        'Nhìn XUỐNG nhẹ'
    ];
    const requiredYaw = [0, -20, 20, 0, 0];
    const tolerance = 5;

    function estimateYaw(landmarks) {
        const leftEye = landmarks.getLeftEye();
        const rightEye = landmarks.getRightEye();
        const nose = landmarks.positions[30];
        const avgLeftX = leftEye.reduce((sum, p) => sum + p.x, 0) / leftEye.length;
        const avgRightX = rightEye.reduce((sum, p) => sum + p.x, 0) / rightEye.length;
        const eyeCenterX = (avgLeftX + avgRightX) / 2;
        const dx = nose.x - eyeCenterX;
        const eyeDist = Math.max(avgRightX - avgLeftX, 1);
        return (dx / eyeDist) * 100;
    }

    async function captureAndSend() {
        const ctx = canvas.getContext('2d');
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

        return new Promise(resolve => {
            canvas.toBlob(async (blob) => {
                if (!blob) {
                    statusDiv.innerText = '⚠️ Không thể tạo ảnh.';
                    resolve();
                    return;
                }
                const formData = new FormData();
                formData.append('image', blob);
                formData.append('idSinhVien', idSinhVien);

                try {
                    const res = await fetch(saveUrl, { method: 'POST', body: formData });
                    const text = await res.text();
                    statusDiv.innerText = res.ok ? '✅ Đã lưu ảnh.' : '❌ ' + text;
                } catch (err) {
                    statusDiv.innerText = '❌ Gửi thất bại: ' + err.message;
                }
                resolve();
            }, 'image/jpeg');
        });
    }

    window.loopCapture = async function () {
        await loadModels();
        const stream = await navigator.mediaDevices.getUserMedia({ video: true });
        video.srcObject = stream;

        const detectOptions = new faceapi.TinyFaceDetectorOptions({ inputSize: 224, scoreThreshold: 0.5 });

        const loop = async () => {
            if (capturedCount >= maxCaptures) {
                instruction.innerText = '';
                statusDiv.innerText = '✅ Đã lưu đủ ảnh.';
                return;
            }

            const detection = await faceapi.detectSingleFace(video, detectOptions).withFaceLandmarks();

            if (!detection) {
                statusDiv.innerText = '👀 Không thấy khuôn mặt.';
                setTimeout(loop, 300);
                return;
            }

            const yaw = estimateYaw(detection.landmarks);
            const expectedYaw = requiredYaw[capturedCount];

            if (Math.abs(yaw - expectedYaw) > tolerance) {
                instruction.innerText = directions[capturedCount];
                statusDiv.innerText = `🔁 ${directions[capturedCount]} (yaw: ${yaw.toFixed(1)})`;
                setTimeout(loop, 400);
                return;
            }

            instruction.innerText = directions[capturedCount];
            statusDiv.innerText = `📸 Chụp ảnh ${capturedCount + 1}/${maxCaptures} (yaw: ${yaw.toFixed(1)})`;

            await captureAndSend();
            capturedCount++;
            setTimeout(loop, 2000);
        };

        loop();
    };
});