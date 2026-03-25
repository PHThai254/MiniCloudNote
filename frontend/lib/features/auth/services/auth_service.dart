import 'dart:convert';
import 'package:flutter/material.dart'; // Chỉ giữ lại để dùng debugPrint
import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:frontend/core/api_config.dart';

class AuthService {
  static String get baseUrl => '${ApiConfig.baseUrl}/auth';

  final _storage = const FlutterSecureStorage();

  // --- Hàm Đăng nhập ---
  // Trả về null nếu thành công. Trả về String (Mã lỗi) nếu thất bại.
  Future<String?> loginUser(String email, String password) async {
    try {
      debugPrint('Đang gửi request Đăng nhập lên Server...');
      final loginUsername = email.split('@')[0];

      final response = await http.post(
        Uri.parse('$baseUrl/login'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({'Username': loginUsername, 'Password': password}),
      );

      if (response.statusCode == 200) {
        final responseData = jsonDecode(response.body);
        final token = responseData['token'];
        await _storage.write(key: 'jwt_token', value: token);
        debugPrint('Đăng nhập thành công! Đã cất Token vào két an toàn!');
        return null; // Thành công
      } else {
        String errorCode = 'UNKNOWN_ERROR';
        try {
          final errorData = jsonDecode(response.body);
          errorCode = errorData['message'] ?? 'UNKNOWN_ERROR';
        } catch (e) {
          errorCode = response.body.replaceAll('"', '').trim();
        }
        if (errorCode.isEmpty) errorCode = 'UNKNOWN_ERROR';
        debugPrint('Lỗi Server: $errorCode');
        return errorCode; // Thất bại: Trả về mã lỗi
      }
    } catch (e) {
      debugPrint('Lỗi mạng: $e');
      return 'NETWORK_ERROR';
    }
  }

  // --- Hàm Đăng ký ---
  // Trả về null nếu thành công. Trả về String (Mã lỗi) nếu thất bại.
  Future<String?> registerUser(
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

      if (response.statusCode == 200) {
        debugPrint('Đăng ký thành công!');
        return null; // Thành công
      } else {
        String errorCode = 'UNKNOWN_ERROR';
        try {
          final errorData = jsonDecode(response.body);
          errorCode =
              errorData['message'] ?? errorData['Message'] ?? 'UNKNOWN_ERROR';
        } catch (e) {
          errorCode = response.body.replaceAll('"', '').trim();
        }
        if (errorCode.isEmpty) errorCode = 'UNKNOWN_ERROR';
        debugPrint('Lỗi Server: $errorCode');
        return errorCode; // Thất bại: Trả về mã lỗi
      }
    } catch (e) {
      debugPrint('Lỗi mạng: $e');
      return 'NETWORK_ERROR';
    }
  }

  // --- Hàm Đăng xuất ---
  // Trả về true nếu xóa token thành công.
  Future<bool> logoutUser() async {
    try {
      debugPrint('Đang thực hiện Đăng xuất...');
      await _storage.delete(key: 'jwt_token');
      debugPrint('Đã xóa Token khỏi két sắt!');
      return true; // Thành công
    } catch (e) {
      debugPrint('Lỗi khi Đăng xuất: $e');
      return false; // Thất bại
    }
  }
}
