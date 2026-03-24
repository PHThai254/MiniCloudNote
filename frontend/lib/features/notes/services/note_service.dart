import 'dart:convert';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:frontend/core/api_config.dart';
import 'package:frontend/features/notes/models/note_model.dart';
import 'package:frontend/features/notes/services/database_helper.dart';
import 'dart:io';
import 'package:http_parser/http_parser.dart'; // Dùng để định nghĩa MediaType

class NoteService {
  // Đường dẫn API lấy danh sách ghi chú (Vẫn dùng 127.0.0.1 và cổng 5265 qua cáp USB)
  static String get baseUrl => '${ApiConfig.baseUrl}/notes';

  final _storage = const FlutterSecureStorage();

  // --- Hàm lấy toàn bộ danh sách Ghi chú (Hỗ trợ Offline) ---
  // THÊM: Tham số searchQuery trong ngoặc nhọn (có thể null)
  // Nếu searchQuery có giá trị thì sẽ được tự động gắn vào URL dưới dạng ?SearchTerm=...
  // --- Hàm Lấy danh sách Ghi chú (Hỗ trợ Offline) ---
  Future<List<Note>> getAllNotes({String? searchQuery}) async {
    try {
      debugPrint('Đang yêu cầu lấy danh sách Ghi chú từ Server...');

      // 1. LẤY TOKEN
      String? token = await _storage.read(key: 'jwt_token');
      if (token == null || token.isEmpty) {
        throw Exception('Chưa đăng nhập!');
      }

      // 2. TẠO URL THÔNG MINH
      String requestUrl = baseUrl;
      if (searchQuery != null && searchQuery.trim().isNotEmpty) {
        requestUrl =
            '$baseUrl?SearchTerm=${Uri.encodeComponent(searchQuery.trim())}';
      }

      // 3. THỬ GỌI API SERVER (CÓ MẠNG)
      try {
        final response = await http
            .get(
              Uri.parse(requestUrl),
              headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer $token',
              },
            )
            .timeout(const Duration(seconds: 5)); // Đợi tối đa 5s

        // --- XỬ LÝ KHI CÓ MẠNG ---
        if (response.statusCode == 200) {
          final dynamic decodedResponse = jsonDecode(response.body);
          List<dynamic> jsonList = [];

          if (decodedResponse is Map<String, dynamic>) {
            jsonList =
                decodedResponse['items'] ?? decodedResponse['data'] ?? [];
          } else if (decodedResponse is List) {
            jsonList = decodedResponse;
          }

          List<Note> notes = jsonList
              .map((json) => Note.fromJson(json))
              .toList();

          // LƯU OFFLINE: Chỉ lưu khi không tìm kiếm
          if (searchQuery == null || searchQuery.trim().isEmpty) {
            debugPrint('Đang lưu ${notes.length} ghi chú vào SQLite...');
            await DatabaseHelper.instance.cacheNotes(notes);
          }

          return notes;
        } else if (response.statusCode == 401) {
          throw Exception('TOKEN_EXPIRED');
        } else {
          throw Exception('Lỗi Server: ${response.statusCode}');
        }
      }
      // 4. BẮT LỖI KHI MẤT MẠNG HOẶC SERVER SẬP
      catch (e) {
        // NẾU LÀ LỖI HẾT HẠN TOKEN THÌ KHÔNG ĐƯỢC LẤY OFFLINE, ĐÁ VĂNG LUÔN
        if (e.toString().contains('TOKEN_EXPIRED')) {
          rethrow;
        }

        // --- BẬT CHẾ ĐỘ OFFLINE ---
        debugPrint('⚠️ Lỗi kết nối Server: $e');
        debugPrint('🔄 Đang lấy dữ liệu từ Kho Offline (SQLite)...');

        final offlineNotes = await DatabaseHelper.instance.getOfflineNotes();

        if (offlineNotes.isNotEmpty) {
          return offlineNotes;
        } else {
          throw Exception('Không có kết nối mạng và chưa có dữ liệu Offline!');
        }
      }
    }
    // 5. BẮT LỖI TỔNG QUÁT BÊN NGOÀI CÙNG
    catch (e) {
      debugPrint('Lỗi NoteService (getAllNotes): $e');
      rethrow;
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

  // --- Hàm Cập nhật Ghi chú (Edit) ---
  Future<bool> updateNote(String id, String title, String content) async {
    try {
      debugPrint('Đang gửi bản cập nhật Ghi chú lên Server (ID: $id)...');

      String? token = await _storage.read(key: 'jwt_token');

      if (token == null || token.isEmpty) throw Exception('Chưa đăng nhập!');

      // Gọi API PUT với đường dẫn có chứa ID ở cuối (VD: /api/notes/a5f23e...)
      final response = await http.put(
        Uri.parse('$baseUrl/$id'), // 1. Đưa Guid lên URL
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
        body: jsonEncode({
          'id': id, // 2. Đưa Guid vào Body (Để khớp với DTO của C#)
          'title': title,
          'content': content,
        }),
      );

      // Backend thường trả về 200 OK hoặc 204 No Content khi update thành công
      if (response.statusCode == 200 || response.statusCode == 204) {
        debugPrint('Cập nhật thành công!');
        return true;
      } else {
        throw Exception(
          'Lỗi Server: ${response.statusCode} - ${response.body}',
        );
      }
    } catch (e) {
      debugPrint('Lỗi NoteService (Cập nhật): $e');
      throw Exception('Không thể cập nhật ghi chú: $e');
    }
  }

  // --- Hàm Xóa Ghi chú (Delete) ---
  Future<bool> deleteNote(String id) async {
    try {
      debugPrint('Đang yêu cầu xóa Ghi chú ID: $id...');

      String? token = await _storage.read(key: 'jwt_token');
      if (token == null || token.isEmpty) throw Exception('Chưa đăng nhập!');

      // Gọi API DELETE
      final response = await http.delete(
        Uri.parse('$baseUrl/$id'),
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      // Backend thường trả về 200 OK hoặc 204 No Content
      if (response.statusCode == 200 || response.statusCode == 204) {
        debugPrint('Đã xóa thành công!');
        return true;
      } else {
        throw Exception('Lỗi Server: ${response.statusCode}');
      }
    } catch (e) {
      debugPrint('Lỗi NoteService (Xóa): $e');
      throw Exception('Không thể xóa: $e');
    }
  }

  // --- Hàm Upload Ảnh và lấy Link trực tiếp ---
  Future<String> uploadImage(File imageFile) async {
    try {
      debugPrint('Đang đóng gói ảnh gửi lên Server...');

      String? token = await _storage.read(key: 'jwt_token');
      if (token == null || token.isEmpty) throw Exception('Chưa đăng nhập!');

      // 1. CHUYẾN XE TẢI CHỞ FILE (POST: /upload)
      var request = http.MultipartRequest('POST', Uri.parse('$baseUrl/upload'));
      request.headers['Authorization'] = 'Bearer $token';

      // Trích xuất đuôi file (jpg, png...)
      String ext = imageFile.path.split('.').last.toLowerCase();
      String subType = 'jpeg'; // Mặc định là jpeg
      if (ext == 'png') {
        subType = 'png';
      } else if (ext == 'gif') {
        subType = 'gif';
      } else if (ext == 'webp') {
        subType = 'webp';
      }
      // Nhấc bức ảnh lên xe. Tên field 'File' phải khớp 100% với tên biến bên C#
      var multipartFile = await http.MultipartFile.fromPath(
        'File',
        imageFile.path,
        contentType: MediaType('image', subType),
      );
      request.files.add(multipartFile);

      // Nhấn ga gửi đi
      var streamedResponse = await request.send();
      var response = await http.Response.fromStream(streamedResponse);

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        // C# trả về: { "fileName": "...", "message": "..." }
        final fileName = data['fileName'] ?? data['FileName'];

        debugPrint('Upload thành công! Tên file trên MinIO: $fileName');

        // 2. CHẠY XE MÁY LÊN XIN LẠI LINK ẢNH (GET: /file/{fileName})
        // Vì C# tách riêng hàm GetFileUrl ra, nên ta gọi luôn để lấy link xịn trả về cho UI
        final linkResponse = await http.get(
          Uri.parse('$baseUrl/file/$fileName'),
          headers: {'Authorization': 'Bearer $token'},
        );

        if (linkResponse.statusCode == 200) {
          final linkData = jsonDecode(linkResponse.body);
          final imageUrl = linkData['url'] ?? linkData['Url'];
          debugPrint('Đã lấy được Link ảnh: $imageUrl');

          return imageUrl; // Trả về link http://... để hiển thị
        } else {
          throw Exception('Upload xong nhưng không lấy được link ảnh!');
        }
      } else {
        throw Exception(
          'Lỗi Upload Server: ${response.statusCode} - ${response.body}',
        );
      }
    } catch (e) {
      debugPrint('Lỗi NoteService (uploadImage): $e');
      throw Exception('Không thể tải ảnh lên: $e');
    }
  }
}
