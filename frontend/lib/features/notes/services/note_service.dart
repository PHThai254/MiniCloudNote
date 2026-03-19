import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:frontend/core/api_config.dart';

// Import "Bản thiết kế" Note mà chúng ta vừa làm
import 'package:frontend/features/notes/models/note_model.dart';

class NoteService {
  // Đường dẫn API lấy danh sách ghi chú (Vẫn dùng 127.0.0.1 và cổng 5265 qua cáp USB)
  static String get baseUrl => '${ApiConfig.baseUrl}/notes';

  final _storage = const FlutterSecureStorage();

  // --- Hàm lấy toàn bộ danh sách Ghi chú ---
  Future<List<Note>> getAllNotes() async {
    try {
      debugPrint('Đang yêu cầu lấy danh sách Ghi chú từ Server...');

      // 1. MỞ KÉT SẮT LẤY CHÌA KHÓA (TOKEN)
      String? token = await _storage.read(key: 'jwt_token');

      // Nếu không có token, báo lỗi ngay lập tức
      if (token == null || token.isEmpty) {
        throw Exception('Chưa đăng nhập hoặc Token đã hết hạn');
      }

      // 2. GỌI API VÀ XUẤT TRÌNH THẺ CĂN CƯỚC (HEADER AUTHORIZATION)
      final response = await http.get(
        Uri.parse(baseUrl),
        headers: {
          'Content-Type': 'application/json',
          // Đây là chiếc vé thông hành! Gắn chữ 'Bearer ' phía trước token
          'Authorization': 'Bearer $token',
        },
      );

      // 3. XỬ LÝ KẾT QUẢ TRẢ VỀ
      if (response.statusCode == 200) {
        // Xóa dòng jsonDecode cũ đi và thay bằng đoạn này:
        final dynamic decodedResponse = jsonDecode(response.body);
        List<dynamic> jsonList = [];

        // Kiểm tra xem C# trả về Hộp (Map - PagedResult) hay Khay (List trực tiếp)
        if (decodedResponse is Map<String, dynamic>) {
          // Trích xuất mảng dữ liệu từ biến 'items' hoặc 'data' bên trong PagedResult
          jsonList = decodedResponse['items'] ?? decodedResponse['data'] ?? [];
        } else if (decodedResponse is List) {
          jsonList = decodedResponse;
        }

        // Đưa mảng lấy được qua dây chuyền lắp ráp Note.fromJson
        List<Note> notes = jsonList.map((json) => Note.fromJson(json)).toList();

        debugPrint('Đã tải thành công ${notes.length} ghi chú!');
        return notes;
      } else if (response.statusCode == 401) {
        throw Exception('Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.');
      } else {
        throw Exception('Lỗi Server: ${response.statusCode}');
      }
    } catch (e) {
      debugPrint('Lỗi NoteService (Lấy danh sách): $e');
      throw Exception('Không thể tải dữ liệu: $e');
    }
  }

  // --- Hàm tạo mới một Ghi chú ---
  Future<bool> createNote(String title, String content) async {
    try {
      debugPrint('Đang gửi Ghi chú mới lên Server...');

      // 1. Mở két sắt lấy Thẻ căn cước (Token)
      String? token = await _storage.read(key: 'jwt_token');

      if (token == null || token.isEmpty) {
        throw Exception('Chưa đăng nhập!');
      }

      // 2. Đóng gói dữ liệu thành JSON và gọi API (POST)
      final response = await http.post(
        Uri.parse(baseUrl),
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token', // Vẫn phải giơ thẻ Bearer ra nhé
        },
        body: jsonEncode({'title': title, 'content': content}),
      );

      // 3. Kiểm tra kết quả (Backend thường trả về 201 Created hoặc 200 OK)
      if (response.statusCode == 200 || response.statusCode == 201) {
        debugPrint('Ghi chú đã được tạo thành công!');
        return true;
      } else {
        debugPrint('Lỗi Server: ${response.statusCode} - ${response.body}');
        throw Exception('Lỗi khi lưu Ghi chú: ${response.statusCode}');
      }
    } catch (e) {
      debugPrint('Lỗi NoteService (Tạo mới): $e');
      throw Exception('Không thể tạo ghi chú: $e');
    }
  }
}
