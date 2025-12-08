# 1. Dùng ảnh gốc Nginx siêu nhẹ
FROM nginx:alpine

# 2. Xóa cấu hình mặc định (cho chắc ăn)
RUN rm /etc/nginx/conf.d/default.conf

# 3. Copy file cấu hình của mình vào đúng chỗ
COPY nginx.conf /etc/nginx/nginx.conf