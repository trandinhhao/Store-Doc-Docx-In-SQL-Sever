CREATE DATABASE DocStorageDB;
GO

USE DocStorageDB;
GO

-- Bảng Documents
CREATE TABLE Documents (
    DocumentId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(500),
    OriginalFormat NVARCHAR(50),
    Author NVARCHAR(255),
    Filesize INT,
    UploadDate DATETIME,
    PublishDate DATETIME,
    FilePath NVARCHAR(255),
);

-- Bảng Sections
CREATE TABLE Sections (
    SectionId INT IDENTITY(1,1) PRIMARY KEY,
    DocumentId INT,
    ParentSectionId INT,
    Level INT,
    Title NVARCHAR(4000),
    OrderInDocument INT,
    FOREIGN KEY (DocumentId) REFERENCES Documents(DocumentId),
    FOREIGN KEY (ParentSectionId) REFERENCES Sections(SectionId)
);

-- Bảng Paragraphs
CREATE TABLE Paragraphs (
    ParagraphId INT IDENTITY(1,1) PRIMARY KEY,
    SectionId INT,
    OrderInSection INT,
    FOREIGN KEY (SectionId) REFERENCES Sections(SectionId)
);

-- Bảng Tables
CREATE TABLE Tables (
    TableId INT IDENTITY(1,1) PRIMARY KEY,
    ParagraphId INT,
    Title NVARCHAR(4000),
    OrderInParagraph INT,
    NumRow INT,
    NumColumn INT,
    FOREIGN KEY (ParagraphId) REFERENCES Paragraphs(ParagraphId)
);

-- Bảng TableCells
CREATE TABLE TableCells (
    TableCellId INT IDENTITY(1,1) PRIMARY KEY,
    TableId INT,
    RowInTable INT,
    ColumnInTable INT,
    FOREIGN KEY (TableId) REFERENCES Tables(TableId)
);

-- Bảng ParagraphSentences
CREATE TABLE ParagraphSentences (
    ParagraphSentenceId INT IDENTITY(1,1) PRIMARY KEY,
    ParagraphId INT,
    OrderInParagraph INT,
    TextContent NVARCHAR(4000),
    FOREIGN KEY (ParagraphId) REFERENCES Paragraphs(ParagraphId),
);

-- Bảng ParagraphEquations
CREATE TABLE ParagraphEquations (
    ParagraphEquationId INT IDENTITY(1,1) PRIMARY KEY,
    ParagraphId INT,
    ParagraphSentenceId INT,
    OrderInParagraph INT,
    OrderInSentence INT,
    EquationContent VARCHAR(255),
    Style VARCHAR(500),
    FOREIGN KEY (ParagraphId) REFERENCES Paragraphs(ParagraphId),
    FOREIGN KEY (ParagraphSentenceId) REFERENCES ParagraphSentences(ParagraphSentenceId)
);

-- Bảng ParagraphImages
CREATE TABLE ParagraphImages (
    ParagraphImageId INT IDENTITY(1,1) PRIMARY KEY,
    ParagraphId INT,
    OrderInParagraph INT,
    ImagePath NVARCHAR(255),
    ImageType NVARCHAR(20),
    Title NVARCHAR(4000),
    Style NVARCHAR(500),
    FOREIGN KEY (ParagraphId) REFERENCES Paragraphs(ParagraphId),
);

-- Bảng TableCellSentences
CREATE TABLE TableCellSentences (
    TableCellSentenceId INT IDENTITY(1,1) PRIMARY KEY,
    TableCellId INT,
    OrderInTableCell INT,
    TextContent NVARCHAR(4000),
    FOREIGN KEY (TableCellId) REFERENCES TableCells(TableCellId),
);

-- Bảng ParagraphEquations
CREATE TABLE TableCellEquations (
    TableCellEquationId INT IDENTITY(1,1) PRIMARY KEY,
    TableCellId INT,
    TableCellSentenceId INT,
    OrderInTableCell INT,
    OrderInSentence INT,
    EquationContent VARCHAR(255),
    Style VARCHAR(500),
    FOREIGN KEY (TableCellId) REFERENCES TableCells(TableCellId),
    FOREIGN KEY (TableCellSentenceId) REFERENCES TableCellSentences(TableCellSentenceId)
);

-- Bảng TableCellImages
CREATE TABLE TableCellImages (
    TableCellImageId INT IDENTITY(1,1) PRIMARY KEY,
    TableCellId INT,
    OrderInTableCell INT,
    ImagePath NVARCHAR(255),
    ImageType NVARCHAR(20),
    Title NVARCHAR(4000),
    Style NVARCHAR(500),
    FOREIGN KEY (TableCellId) REFERENCES TableCells(TableCellId),
);
