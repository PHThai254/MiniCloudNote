import 'package:sqflite/sqflite.dart';
import 'package:path/path.dart';
import 'package:frontend/features/notes/models/note_model.dart';

class DatabaseHelper {
  // Biến class thành Singleton (Chỉ có duy nhất 1 người quản lý kho trong toàn bộ App)
  static final DatabaseHelper instance = DatabaseHelper._init();
  static Database? _database;

  DatabaseHelper._init();

  // Hàm mở cửa kho (Nếu kho chưa có thì xây mới)
  Future<Database> get database async {
    if (_database != null) return _database!;
    _database = await _initDB('minicloudnote_offline.db');
    return _database!;
  }

  // Hàm tìm đất và xây kho
  Future<Database> _initDB(String filePath) async {
    final dbPath = await getDatabasesPath();
    final path = join(dbPath, filePath);

    // Mở database, nếu file chưa tồn tại sẽ gọi hàm _createDB
    return await openDatabase(path, version: 1, onCreate: _createDB);
  }

  // Hàm xây các "Kệ hàng" (Tạo Bảng)
  Future _createDB(Database db, int version) async {
    // Định nghĩa kiểu dữ liệu cho SQLite
    const idType =
        'TEXT PRIMARY KEY'; // Nhớ chứ? C# dùng Guid nên ID bên SQLite bắt buộc phải là TEXT (String)
    const textType = 'TEXT NOT NULL';
    const textNullable = 'TEXT';

    // Xây bảng 'notes' khớp 100% với model Note của bạn
    await db.execute('''
      CREATE TABLE notes (
        id $idType,
        title $textType,
        content $textType,
        createdAt $textNullable
      )
    ''');
  }

  // --- CÁC HÀM THAO TÁC VỚI KHO ---

  // 1. Cất hàng loạt ghi chú vào kho (Dùng khi vừa tải từ C# về)
  Future<void> cacheNotes(List<Note> notes) async {
    final db = await instance.database;

    // Xóa sạch kho cũ trước khi nhập lô hàng mới cho đỡ rác
    await db.delete('notes');

    // Cất từng cái vào kho
    for (var note in notes) {
      await db.insert(
        'notes',
        {
          'id': note.id,
          'title': note.title,
          'content': note.content,
          'createdAt': note.createdAt?.toIso8601String(),
        },
        // Nếu trùng ID thì ghi đè lên cái cũ
        conflictAlgorithm: ConflictAlgorithm.replace,
      );
    }
  }

  // 2. Lấy toàn bộ hàng trong kho ra (Dùng khi mất mạng)
  Future<List<Note>> getOfflineNotes() async {
    final db = await instance.database;

    // Đọc bảng notes, sắp xếp theo thời gian tạo mới nhất
    final result = await db.query('notes', orderBy: 'createdAt DESC');

    // Chuyển từ dạng Map của SQLite sang dạng List<Note> quen thuộc của Flutter
    return result
        .map(
          (json) => Note(
            id: json['id'] as String?,
            title: json['title'] as String,
            content: json['content'] as String,
            createdAt: json['createdAt'] != null
                ? DateTime.parse(json['createdAt'] as String)
                : null,
          ),
        )
        .toList();
  }

  // 3. Đốt kho (Dùng khi người dùng bấm Đăng xuất)
  Future<void> clearAll() async {
    final db = await instance.database;
    await db.delete('notes');
  }
}
