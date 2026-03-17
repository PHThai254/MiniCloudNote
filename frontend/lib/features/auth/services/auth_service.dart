import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:frontend/features/auth/screens/login_screen.dart';
import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart'; // Thư viện để lưu trữ an toàn (Token)

class AuthService {
  static const String baseUrl = 'http://127.0.0.1:5265/api/auth';

  // TẠO KÉT SẮT BẢO MẬT: Sẽ dùng để lưu Token
  final _storage = const FlutterSecureStorage();

  // --- Hàm Đăng nhập ---
  // Đổi từ Future<void> thành Future<bool> để báo cho UI biết là Đăng nhập thành công hay thất bại
  Future<bool> loginUser(
    BuildContext context,
    String email,
    String password,
  ) async {
    try {
      debugPrint('Đang gửi request Đăng nhập lên Server...');
      debugPrint('>>> SỰ THẬT: Đang gọi vào URL: $baseUrl/login');

      // Ép Frontend tự động trích xuất Username từ Email
      // pht1234@example.com -> pht1234
      final loginUsername = email.split('@')[0];

      // Đóng gói dữ liệu thành định dạng JSON
      final response = await http.post(
        Uri.parse('$baseUrl/login'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({'Username': loginUsername, 'Password': password}),
      );

      // Tấm khiên bảo vệ context
      if (!context.mounted) return false;

      // Kiểm tra kết quả Server trả về
      if (response.statusCode == 200) {
        // BƯỚC 1: BÓC LẤY TOKEN TỪ SERVER TRẢ VỀ
        final responseData = jsonDecode(response.body);
        final token = responseData['token']; // Chìa khóa Backend C# trả về

        // BƯỚC 2: CẤT VÀO KÉT SẮT
        await _storage.write(key: 'jwt_token', value: token);

        debugPrint('Đăng nhập thành công! Đã cất Token vào két an toàn!');

        return true; // Trả về true để màn hình Login biết đường chuyển trang
      } else {
        // BÓC TÁCH LỚP VỎ JSON LỖI
        String errorMessage = 'Đã xảy ra lỗi không xác định';
        try {
          final errorData = jsonDecode(response.body);
          errorMessage = errorData['message'] ?? response.body;
        } catch (e) {
          errorMessage = response.body;
        }

        debugPrint('Lỗi Server: $errorMessage');

        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(errorMessage), backgroundColor: Colors.red),
        );

        return false; // Đăng nhập thất bại
      }
    } catch (e) {
      debugPrint('Lỗi mạng: $e');

      if (!context.mounted) return false;

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Không thể kết nối đến máy chủ. Vui lòng kiểm tra mạng!',
          ),
          backgroundColor: Colors.red,
        ),
      );

      return false; // Lỗi mạng cũng là thất bại
    }
  }

  // --- Hàm Đăng ký ---
  Future<bool> registerUser(
    BuildContext context,
    String username,
    String fullName,
    String email,
    String password,
  ) async {
    try {
      debugPrint('Đang gửi request Đăng ký lên Server...');

      final response = await http.post(
        Uri.parse('$baseUrl/register'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'UserName': username,
          'FullName': fullName,
          'Email': email,
          'Password': password,
        }),
      );

      if (!context.mounted) return false;

      if (response.statusCode == 200) {
        debugPrint('Đăng ký thành công!');
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Đăng ký thành công! Vui lòng đăng nhập.'),
            backgroundColor: Colors.green,
          ),
        );
        return true;
      } else {
        String errorMessage = 'Đã xảy ra lỗi không xác định';
        try {
          final errorData = jsonDecode(response.body);
          errorMessage = errorData['message'] ?? response.body;
        } catch (e) {
          errorMessage = response.body;
        }

        debugPrint('Lỗi Server: $errorMessage');
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Đăng ký thất bại: $errorMessage'),
            backgroundColor: Colors.red,
          ),
        );
        return false;
      }
    } catch (e) {
      debugPrint('Lỗi mạng: $e');
      if (!context.mounted) return false;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Không thể kết nối đến máy chủ. Vui lòng kiểm tra mạng!',
          ),
          backgroundColor: Colors.red,
        ),
      );
      return false;
    }
  }

  // --- Hàm Đăng xuất ---
  Future<void> logoutUser(BuildContext context) async {
    try {
      debugPrint('Đang thực hiện Đăng xuất...');

      // 1. Mở két sắt và xóa Token
      await _storage.delete(key: 'jwt_token');
      debugPrint('Đã xóa Token khỏi két sắt!');

      // Tam khiên bảo vệ context
      if (!context.mounted) return;

      // 2.Chuyển hướng về LoginScreen và XÓA SẠCH lịch sử trang
      // pushAndRemoveUntil giúp hủy toàn bộ các màn hình trước đó
      // Tránh việc người dùng bấm phím Back (trở về) trên điện thoại và lọt lại vào HomeScreen
      Navigator.pushAndRemoveUntil(
        context,
        MaterialPageRoute(builder: (context) => const LoginScreen()),
        (route) => false, // false nghĩa là xóa hết mọi route cũ
      );
    } catch (e) {
      debugPrint('Lỗi khi Đăng xuất: $e');
      if (!context.mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Đã xảy ra lỗi khi đăng xuất. Vui lòng thử lại!'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }
}
