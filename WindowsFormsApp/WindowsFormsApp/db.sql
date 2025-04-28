CREATE DATABASE DocStorageDB;
GO

USE DocStorageDB;
GO

-- 1. Tài liệu -- OK
CREATE TABLE Document (
    DocumentId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(500),
    Author NVARCHAR(255),
    OriginalFormat NVARCHAR(10),
    OriginalFileName NVARCHAR(255),
    FileSize BIGINT NULL,
    UploadDate DATETIME DEFAULT GETDATE(),
    FilePath NVARCHAR(500) NULL,
    PublishDate DATETIME NULL,
    HtmlContent NVARCHAR(MAX) NULL,
);

-- 2. Mục nội dung -- OK
CREATE TABLE Section (
    SectionId INT IDENTITY(1,1) PRIMARY KEY,
    DocumentId INT FOREIGN KEY REFERENCES Document(DocumentId),
    ParentSectionId INT NULL FOREIGN KEY REFERENCES Section(SectionId),
    Title NVARCHAR(500),
    Level INT,
    OrderInDocument INT NOT NULL
);

-- 3. Đoạn văn -- OK
CREATE TABLE Paragraph (
    ParagraphId INT IDENTITY(1,1) PRIMARY KEY,
    SectionId INT FOREIGN KEY REFERENCES Section(SectionId),
    OrderInSection INT NOT NULL,
    Alignment NVARCHAR(50) NULL
);

-- 4. Câu -- OK
CREATE TABLE Sentence (
    SentenceId INT IDENTITY(1,1) PRIMARY KEY,
    ParagraphId INT FOREIGN KEY REFERENCES Paragraph(ParagraphId),
    OrderInParagraph INT NOT NULL,
    TextContent NVARCHAR(MAX),
    Font NVARCHAR(100) NULL,
    FontSize INT NULL,
    Bold BIT DEFAULT 0,
    Italic BIT DEFAULT 0,
    Underline BIT DEFAULT 0,
    Color NVARCHAR(50) NULL
);

-- 5. Bảng -- OK
CREATE TABLE TableElement (
    TableId INT IDENTITY(1,1) PRIMARY KEY,
    ParagraphId INT FOREIGN KEY REFERENCES Paragraph(ParagraphId),
    TableHTML NVARCHAR(MAX) NULL,
    OrderInParagraph INT NOT NULL
);

-- 6. Hình ảnh -- OK
CREATE TABLE Image (
    ImageId INT IDENTITY(1,1) PRIMARY KEY,
    ParagraphId INT FOREIGN KEY REFERENCES Paragraph(ParagraphId),
    SentenceId INT NULL FOREIGN KEY REFERENCES Sentence(SentenceId),
    OrderInParagraph INT NOT NULL,
    OrderInSentence INT NULL,
    ImageHTML NVARCHAR(MAX) NULL
);

-- 7. Công thức -- OK
CREATE TABLE Equation (
    EquationId INT IDENTITY(1,1) PRIMARY KEY,
    ParagraphId INT FOREIGN KEY REFERENCES Paragraph(ParagraphId),
    SentenceId INT NULL FOREIGN KEY REFERENCES Sentence(SentenceId),
    EquationHTML NVARCHAR(MAX) NULL,
    OrderInParagraph INT NOT NULL,
    OrderInSentence INT NULL
);