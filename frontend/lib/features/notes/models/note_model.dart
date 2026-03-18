class Note {
  final int?
  id; // Dấu ? vì khi tạo Note mới trên app, nó chưa có ID (C# sẽ tự cấp)
  final String title;
  final String content;
  final DateTime? createdAt;

  Note({this.id, required this.title, required this.content, this.createdAt});

  // --- HÀM 1: Nhận hàng từ C# (Từ JSON -> Dart Object) ---
  // ASP.NET Core mặc định trả về JSON dạng camelCase (chữ cái đầu viết thường)
  factory Note.fromJson(Map<String, dynamic> json) {
    // Xử lý an toàn cho ID: Cho dù Backend trả về số (1) hay chuỗi ("1") thì cũng ép về int được
    int? parsedId;
    if (json['id'] != null) {
      if (json['id'] is int) {
        parsedId = json['id'];
      } else if (json['id'] is String) {
        parsedId = int.tryParse(json['id']);
      }
    }

    return Note(
      id: parsedId,
      title:
          json['title'] ??
          'Không có tiêu đề', // Nếu null thì gán giá trị mặc định
      content: json['content'] ?? '',
      // Chuyển chuỗi thời gian của C# thành đối tượng DateTime của Dart
      createdAt: json['createdAt'] != null
          ? DateTime.parse(json['createdAt'])
          : null,
    );
  }

  // --- HÀM 2: Gửi hàng lên C# (Từ Dart Object -> JSON) ---
  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'title': title,
      'content': content,
      // Khi gửi lên C# thường gửi dạng chuỗi ISO 8601
      'createdAt': createdAt?.toIso8601String(),
    };
  }
}
