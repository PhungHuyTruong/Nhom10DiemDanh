window.initAttendance = async function (recognizeUrl) {
    console.log("🟢 Hàm initAttendance đã được gọi với URL:", recognizeUrl);
    const video = document.getElementById('video');
    const canvas = document.getElementById('canvas');
    const statusDiv = document.getElementById('status');
    const resultDiv = document.getElementById('result');

    await faceapi.nets.tinyFaceDetector.loadFromUri('/models');
    await faceapi.nets.faceLandmark68Net.loadFromUri('/models');
    console.log("✅ Models loaded for attendance");

    try {
        const stream = await navigator.mediaDevices.getUserMedia({ video: true });
        video.srcObject = stream;
        await new Promise(resolve => video.onloadedmetadata = resolve);
    } catch (err) {
        statusDiv.innerText = "🚫 Không thể mở camera: " + err.message;
        return;
    }

    const detectOptions = new faceapi.TinyFaceDetectorOptions({ inputSize: 224, scoreThreshold: 0.5 });

    const loop = async () => {
        const detection = await faceapi.detectSingleFace(video, detectOptions);

        if (!detection) {
            statusDiv.innerText = "🔍 Không thấy khuôn mặt.";
            setTimeout(loop, 1000);
            return;
        }

        statusDiv.innerText = "📸 Đang nhận diện...";

        const ctx = canvas.getContext('2d');
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

        canvas.toBlob(async (blob) => {
            if (!blob) {
                resultDiv.innerText = "❌ Không thể tạo ảnh.";
                return;
            }

            const formData = new FormData();
            formData.append('image', blob);

            try {
                const res = await fetch(recognizeUrl, {
                    method: 'POST',
                    body: formData
                });

                const text = await res.text();
                resultDiv.innerText = text;

                if (res.ok && text.includes("✅")) {
                    const match = text.match(/sinh viên: (\-?\d+)/);
                    if (match && match[1]) {
                        const idHash = match[1];
                        const res2 = await fetch('/DiemDanh/LuuDiemDanh?idHash=' + idHash, { method: 'POST' });
                        const msg = await res2.text();
                        console.log("✅ Đã điểm danh:", msg);

                        // ✅ Dừng vòng lặp tại đây
                        statusDiv.innerText = "🎉 Đã điểm danh xong!";
                        return;
                    }
                }

                // ❌ Nếu không thành công thì tiếp tục vòng lặp
                setTimeout(loop, 3000);
            } catch (err) {
                resultDiv.innerText = "❌ Lỗi nhận diện: " + err.message;
                setTimeout(loop, 3000);
            }
        }, 'image/jpeg');
    };

    loop();
};

window.addEventListener('DOMContentLoaded', () => {
    const video = document.getElementById('video');
    const canvas = document.getElementById('canvas');
    const statusDiv = document.getElementById('status');
    const instruction = document.getElementById('instruction');
    let capturedCount = 0;
    const maxCaptures = 5;

    const idSinhVien = '@ViewBag.IdSinhVien';
    const saveUrl = '/Face/SaveFace';

    const directions = [
        "Hãy nhìn THẲNG vào camera",
        "Quay mặt sang TRÁI",
        "Quay mặt sang PHẢI",
        "Nhìn LÊN một chút",
        "Nhìn XUỐNG nhẹ"
    ];
    const requiredYaw = [0, -20, 20, 0, 0];  // yaw yêu cầu lớn hơn
    const tolerance = 5;                    // nghiêm ngặt hơn


    async function loadModels() {
        await faceapi.nets.tinyFaceDetector.loadFromUri('/models');
        await faceapi.nets.faceLandmark68Net.loadFromUri('/models');
        console.log("✅ Models loaded");
    }

    async function startCamera() {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ video: true });
            video.srcObject = stream;
            return new Promise(resolve => video.onloadedmetadata = resolve);
        } catch (error) {
            statusDiv.innerText = "🚫 Không thể truy cập camera: " + error.message;
        }
    }

    function estimateYaw(landmarks) {
        const leftEye = landmarks.getLeftEye();
        const rightEye = landmarks.getRightEye();
        const nose = landmarks.positions[30]; // điểm giữa mũi

        const avgLeftX = leftEye.reduce((sum, p) => sum + p.x, 0) / leftEye.length;
        const avgRightX = rightEye.reduce((sum, p) => sum + p.x, 0) / rightEye.length;

        const eyeCenterX = (avgLeftX + avgRightX) / 2;
        const dx = nose.x - eyeCenterX;
        const eyeDist = Math.max(avgRightX - avgLeftX, 1); // tránh chia 0

        const yaw = (dx / eyeDist) * 100;
        return yaw;
    }

    function captureAndSend(idSinhVien, saveUrl) {
        const ctx = canvas.getContext('2d');
        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;
        ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

        return new Promise(resolve => {
            canvas.toBlob(async (blob) => {
                if (!blob) {
                    statusDiv.innerText = "⚠️ Không thể tạo ảnh.";
                    resolve();
                    return;
                }

                const formData = new FormData();
                formData.append('image', blob);
                formData.append('idSinhVien', idSinhVien);

                try {
                    const res = await fetch(saveUrl, {
                        method: 'POST',
                        body: formData
                    });
                    const text = await res.text();
                    console.log("📤 Server:", text);

                    statusDiv.innerText = res.ok ? "✅ Đã lưu ảnh." : "❌ " + text;
                } catch (err) {
                    statusDiv.innerText = "❌ Gửi thất bại: " + err.message;
                }

                resolve();
            }, 'image/jpeg');
        });
    }

    window.loopCapture = async function (idSinhVien, saveUrl) {
        await loadModels();
        await startCamera();

        const detectOptions = new faceapi.TinyFaceDetectorOptions({ inputSize: 224, scoreThreshold: 0.5 });

        const loop = async () => {
            if (capturedCount >= maxCaptures) {
                instruction.innerText = "";
                statusDiv.innerText = "✅ Đã lưu đủ ảnh.";
                return;
            }

            const detection = await faceapi.detectSingleFace(video, detectOptions).withFaceLandmarks();

            if (!detection) {
                statusDiv.innerText = "👀 Không thấy khuôn mặt.";
                setTimeout(loop, 300);
                return;
            }

            const yaw = estimateYaw(detection.landmarks);
            const expectedYaw = requiredYaw[capturedCount];

            console.log(`🔍 Yaw hiện tại: ${yaw.toFixed(1)} | Yêu cầu: ${expectedYaw}`);

            if (Math.abs(yaw - expectedYaw) > tolerance) {
                instruction.innerText = directions[capturedCount];
                statusDiv.innerText = `🔁 ${directions[capturedCount]} (yaw: ${yaw.toFixed(1)})`;
                setTimeout(loop, 400);
                return;
            }

            instruction.innerText = directions[capturedCount];
            statusDiv.innerText = `📸 Chụp ảnh ${capturedCount + 1}/${maxCaptures} (yaw: ${yaw.toFixed(1)})`;

            await captureAndSend(idSinhVien, saveUrl);
            capturedCount++;
            setTimeout(loop, 2000); // thời gian đổi góc
        };

        loop();
    };
});
