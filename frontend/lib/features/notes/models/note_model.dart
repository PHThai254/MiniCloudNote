class Note {
  final String? id; // Chắc chắn đây là String? vì C# dùng Guid
  final String title;
  final String content;
  final DateTime? createdAt;
  final DateTime? updatedAt; // Ngày chỉnh sửa gần nhất
  final bool isPinned;

  Note({
    this.id,
    required this.title,
    required this.content,
    this.createdAt,
    this.updatedAt,
    this.isPinned = false,
  });

  // --- HÀM 1: Nhận hàng từ C# (Từ JSON -> Dart Object) ---
  factory Note.fromJson(Map<String, dynamic> json) {
    return Note(
      // Ép thẳng sang chuỗi (String) vì Backend dùng Guid
      id: json['id']?.toString(),
      title: json['title'] ?? '', // Nếu null thì gán giá trị mặc định
      content: json['content'] ?? '',

      // Phân tích Ngày Tạo: Chuyển chuỗi thời gian của C# thành đối tượng DateTime của Dart
      createdAt: json['createdAt'] != null
          ? DateTime.parse(json['createdAt'])
          : null,
      // Phân tích Ngày Cập Nhật (Nếu không có updatedAt, lấy tạm createdAt)
      updatedAt: json['updatedAt'] != null
          ? DateTime.parse(json['updatedAt'])
          : (json['createdAt'] != null
                ? DateTime.parse(json['createdAt'])
                : null),
      isPinned: json['isPinned'] ?? false, // ĐỌC DỮ LIỆU TỪ JSON C# GỬI VỀ
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
      'updatedAt': updatedAt?.toIso8601String(),
      'isPinned': isPinned, // GÓI VÀO JSON NẾU CẦN
    };
  }
}
