import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;

class AuthService {
  static const String baseUrl = 'http://172.20.36.152:5265/api/auth';

  // --- Hàm Đăng nhập ---
  Future<void> loginUser(
    BuildContext context,
    String email,
    String password,
  ) async {
    try {
      debugPrint('Đang gửi request Đăng nhập lên Server...');
      debugPrint('>>> SỰ THẬT: Đang gọi vào URL: $baseUrl/login');

      // Đóng gói dữ liệu thành định dạng JSON
      final response = await http.post(
        Uri.parse('$baseUrl/login'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({'Username': email, 'Password': password}),
      );

      // 1. TẤM KHIÊN BẢO VỆ CONTEXT: Giúp xóa sạch mọi gạch chân màu xanh lam
      // Dòng này báo với Flutter: "Nếu người dùng đã thoát màn hình này rồi thì bỏ qua luôn"
      if (!context.mounted) return;

      // Kiểm tra kết quả Server trả về
      if (response.statusCode == 200) {
        debugPrint('Đăng nhập thành công! Token: ${response.body}');
        // TODO: Lưu Token vào máy và chuyển sang màn hình Home
      } else {
        // 2. BÓC TÁCH LỚP VỎ JSON
        String errorMessage = 'Đã xảy ra lỗi không xác định';
        try {
          // Dịch cục JSON text thành một Map (Từ điển) trong Dart
          final errorData = jsonDecode(response.body);

          // Trích xuất đúng câu chữ nằm trong chìa khóa "message"
          errorMessage = errorData['message'] ?? response.body;
        } catch (e) {
          // Phòng hờ trường hợp rớt mạng, server sập trả về HTML thay vì JSON
          errorMessage = response.body;
        }

        debugPrint('Lỗi Server: $errorMessage');

        // Hiện lỗi lên màn hình cho người dùng biết
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              errorMessage,
            ), // Chỗ này giờ chỉ còn là chữ tiếng Việt sạch sẽ
            backgroundColor: Colors.red,
          ),
        );
      }
    } catch (e) {
      debugPrint('Lỗi mạng: $e');

      // Tấm khiên bảo vệ cho khối catch
      if (!context.mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Không thể kết nối đến máy chủ. Vui lòng kiểm tra mạng!',
          ),
          backgroundColor: Colors.red,
        ),
      );
    }
  }
}
