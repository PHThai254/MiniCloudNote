class Note {
  final String? id; // Chắc chắn đây là String? vì C# dùng Guid
  final String title;
  final String content;
  final DateTime? createdAt;

  Note({this.id, required this.title, required this.content, this.createdAt});

  // --- HÀM 1: Nhận hàng từ C# (Từ JSON -> Dart Object) ---
  // ASP.NET Core mặc định trả về JSON dạng camelCase (chữ cái đầu viết thường)
  factory Note.fromJson(Map<String, dynamic> json) {
    return Note(
      // Ép thẳng sang chuỗi (String) vì Backend dùng Guid
      id: json['id']?.toString(),
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
