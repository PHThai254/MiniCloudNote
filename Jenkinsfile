pipeline {
    agent none

    stages {
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
    }
}