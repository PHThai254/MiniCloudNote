# 1. Kế thừa từ bản gốc ổn định
FROM jenkins/jenkins:lts

# 2. Chuyển sang quyền Root để cài phần mềm
USER root 

# 3. Cài đặt Docker CLI (Bộ gõ lệnh Docker)
# Lưu ý: Ta chỉ cài CLI để nó "ra lệnh" ra bên ngoài, không cài Docker Engine
RUN apt-get update && \
    apt-get install -y docker.io && \
    rm -rf /var/lib/apt/list/*

# 4. Trả lại quyền cho user jenkins (Bảo mật)
USER jenkins
