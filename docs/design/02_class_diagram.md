# Sơ đồ Lớp (Class Diagram) - Domain Layer

## 1. Tổng quan
Tài liệu này mô tả Sơ đồ lớp (Class Diagram) cho tầng **Core (Domain Layer)** của dự án MiniCloudNote. Thiết kế này tuân thủ nghiêm ngặt nguyên lý **Rich Domain Model** và tính đóng gói (Encapsulation) trong Lập trình hướng đối tượng (OOP).

## 2. Sơ đồ Minh họa
*(Bấm vào ảnh để xem kích thước đầy đủ)*

![Sơ đồ lớp (Class Diagram)](assets/class_diagram.png)

## 3. Phân tích Các Thực thể (Entities)

### 3.1. Lớp Nền tảng (Base)
* **`BaseEntity`**: Lớp cha trừu tượng mà mọi thực thể trong hệ thống đều kế thừa. Cung cấp các thuộc tính cốt lõi để theo dõi vòng đời dữ liệu (Audit Trail) bao gồm: `Id`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, và cờ `IsDeleted` phục vụ cơ chế Xóa mềm (Soft Delete).

### 3.2. Nhóm Quản lý Người dùng (Identity & Profile)
Hệ thống áp dụng kỹ thuật phân tách (Separation) để tối ưu hóa truy vấn Database:
* **`ApplicationUser`**: Chứa dữ liệu nhạy cảm phục vụ xác thực (`Email`, `PasswordHash`, `Role`). Các thuộc tính này được đóng gói (Private) và chỉ có thể thay đổi qua các hành vi (Public Methods) như `ValidatePassword()` hay `ChangePassword()`.
* **`UserProfile`**: Chứa dữ liệu hiển thị cá nhân (`FullName`, `AvatarUrl`, `Bio`...). Mối quan hệ với `ApplicationUser` là Composition `1-1` (Một User sở hữu đúng một Profile).

### 3.3. Nhóm Quản lý Ghi chú (Core Domain)
* **`Note`**: Thực thể trung tâm của ứng dụng. Lưu trữ nội dung ghi chú và các cờ trạng thái (`IsPinned`, `IsArchived`, `IsInTrash`). Dữ liệu được bảo vệ nghiêm ngặt và chỉ thay đổi thông qua các phương thức nghiệp vụ như `MoveToTrash()`, `Pin()`, `Restore()`.
* **`NoteAttachment`**: Quản lý siêu dữ liệu (Metadata) của các tệp đính kèm (`FileName`, `FileUrl`, `FileSize`). Mối quan hệ với `Note` là Composition `1-n` (Một Note chứa nhiều Attachments, khi Note bị xóa, các Attachments cũng bị tiêu hủy theo).

### 3.4. Danh mục (Enums)
* **`UserRole`**: Định nghĩa quyền hạn hệ thống (`Customer`, `Admin`).
* **`NoteType`**: Phân loại định dạng ghi chú (`Standard`, `Checklist`, `CodeSnippet`).

## 4. Ghi chú Kỹ thuật (Technical Notes)
* **Quy ước UML**: 
  * Dấu `-` (Private): Tất cả các trường dữ liệu (Attributes) đều bị khóa để bảo vệ tính toàn vẹn.
  * Dấu `+` (Public): Các phương thức (Operations) thể hiện hành vi cho phép tương tác từ bên ngoài.
  * Mũi tên tam giác rỗng: Quan hệ kế thừa (Inheritance).
  * Hình thoi đen: Quan hệ sở hữu chặt chẽ (Composition).
* Các lớp trong sơ đồ này sẽ được ánh xạ (mapping) trực tiếp thành các bảng trong PostgreSQL thông qua Entity Framework Core.