import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

// Import "Bản thiết kế" Note mà chúng ta vừa làm
import 'package:frontend/features/notes/models/note_model.dart';

class NoteService {
  // Đường dẫn API lấy danh sách ghi chú (Vẫn dùng 127.0.0.1 và cổng 5265 qua cáp USB)
  static const String baseUrl = 'http://127.0.0.1:5265/api/notes';

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
}
