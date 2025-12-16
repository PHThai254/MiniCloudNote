pipeline {
    agent none

    stages {
        // 0. chạy Test
        stage('Unit Test') {
            agent {
                docker {
                    // Dùng Image SDK để có lệnh 'dotnet test'
                    image 'mcr.microsoft.com/dotnet/sdk:9.0'
                    // Mount thư mục hiện tại vào container để thấy code
                    args '-v $PWD:/app -w /app'
                }
            }
            steps {
                // /p:CollectCoverage=true : Bật chế độ đo
                // /p:Threshold=1          : Nếu độ phủ dưới 1% thì coi như LỖI (Build Failed)
                sh 'dotnet test src/MiniCloudNote.Tests/MiniCloudNote.Tests.csproj /p:CollectCoverage=true /p:Threshold=0'
            }
        }
        // 1. Lấy code về (Dùng agent nào cũng được, chọn 'any' cho nhanh)
        stage('Checkout') {
            agent any
            steps {
                // Thay URL dưới đây bằng link GitHub repo của bạn
                git branch: 'main', url: 'https://github.com/PHThai254/MiniCloudNote.git'
            }
        }

        // 2. Build code thật
        stage('Build .NET') {
            agent {
                docker {
                    image 'mcr.microsoft.com/dotnet/sdk:9.0'
                }
            }
            steps {
                // Chạy lệnh thật trên code vừa tải về
                sh 'dotnet restore ./src/MiniCloudNote.API/MiniCloudNote.API.csproj'
                sh 'dotnet build --no-restore ./src/MiniCloudNote.API/MiniCloudNote.API.csproj'
                
                // Chạy luôn cả Test (cho máu!)
                sh 'dotnet test --no-build ./src/MiniCloudNote.UnitTests/MiniCloudNote.UnitTests.csproj'
            }
        }

        // 3. Đóng gói Docker (Build Image)
        stage('Docker Build') {
            agent {
                docker {
                    image 'docker'
                    args '-v /var/run/docker.sock:/var/run/docker.sock'
                }
            }
            steps {
                script {
                    // Tạo tên ảnh kèm tag thời gian để không bị trùng
                    def imageTag = "minicloudnote:jenkins-${env.BUILD_ID}"
                    
                    // Lệnh build image thật
                    sh "docker build -t ${imageTag} -f ./src/MiniCloudNote.API/Dockerfile ."
                    
                    echo "Build thanh cong image: ${imageTag}"
                }
            }
        }
        // 4. TRIỂN KHAI (CD) - Chạy thử trên cổng 5050
        stage('Deploy Staging') {
            // Khai báo biến môi trường lấy từ kho bí mật
            environment {
                // Biến DB_STRING sẽ chứa giá trị thật, nhưng Jenkins sẽ giấu nó đi
                DB_STRING = credentials('staging-db-conn-string')
            }
            agent {
                docker {
                    image 'docker'
                    args '-v /var/run/docker.sock:/var/run/docker.sock'
                }
            }
            steps {
                script {
                    def containerName = "minicloud-staging"
                    def port = "5050" // Cổng cho bản chạy thử

                    // Lấy tag vừa build ở bước trước (cần đồng bộ biến này)
                    // Để đơn giản cho bài học, ta hardcode hoặc dùng biến môi trường.
                    // Ở đây ta dùng lại logic đặt tên:
                    def imageTag = "minicloudnote:jenkins-${env.BUILD_ID}"

                    echo "Deploying ${imageTag} to Staging..."

                    // 1. Dọn dẹp: Nếu có bản staging cũ đang chạy thì tắt và xóa đi
                    // Dùng '|| true' để không báo lỗi nếu container chưa tồn tại
                    sh "docker stop ${containerName} || true"
                    sh "docker rm ${containerName} || true"

                    // 2. Chạy container mới
                    // --network minicloud-network: Để nó kết nối được với DB, Redis cũ
                    sh """
                        docker run -d \
                        --name ${containerName} \
                        --network minicloudnote_minicloud-network \
                        -p ${port}:8080 \
                        -e ConnectionStrings__DefaultConnection="$DB_STRING" \
                        -e REDIS_CONNECTION='minicloud-redis:6379' \
                        -e ASPNETCORE_ENVIRONMENT=Development \
                        ${imageTag}
                    """       
                }
            }
        }
    }

    post {
        success {
            script {
                discordSend("✅ BUILD THÀNH CÔNG!", "3066993") // Màu xanh
            }
        }
        failure {
            script {
                discordSend("❌ BUILD THẤT BẠI!", "15158332") // Màu Đỏ
            }
        }
    }
}

// Hàm gửi tin nhắn
def discordSend(String title, String color) {
    // Thêm 'node' để Jenkins cấp một executor chạy lệnh này
    node {
        withCredentials([string(credentialsId: 'discord-webhook-url', variable: 'DISCORD_URL')]) {
            // Thêm dấu \ trước biến DISCORD_URL để sửa lỗi bảo mật (để Shell tự xử lý thay vì Groovy)
            sh """
                curl -H "Content-Type: application/json" \
                -X POST \
                -d '{
                    "username": "Jenkins Master",
                    "embeds": [{
                        "title": "${title}",
                        "description": "Job: ${env.JOB_NAME} - Build #${env.BUILD_NUMBER}\\n[Xem chi tiết](${env.BUILD_URL})",
                        "color": ${color}
                    }]     
                }' \
                "\$DISCORD_URL"
            """ 
        }   
    }
}