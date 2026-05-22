-- ============================================================
-- HangmanGame: Create game-related tables
-- Run this script on HangmanGameDB after Users table exists
-- ============================================================
USE HangmanGameDB;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
CREATE TABLE Categories (
    CategoryId   INT           PRIMARY KEY IDENTITY(1,1),
    Name         NVARCHAR(100) NOT NULL,
    LanguageCode NVARCHAR(5)   NOT NULL DEFAULT 'ES'
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Words')
CREATE TABLE Words (
    WordId       INT           PRIMARY KEY IDENTITY(1,1),
    CategoryId   INT           NOT NULL REFERENCES Categories(CategoryId),
    Text         NVARCHAR(100) NOT NULL,
    Hint         NVARCHAR(300) NULL,
    LanguageCode NVARCHAR(5)   NOT NULL DEFAULT 'ES',
    Difficulty   INT           NOT NULL DEFAULT 1  -- 1=Facil 2=Medio 3=Dificil
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Games')
CREATE TABLE Games (
    GameId             INT      PRIMARY KEY IDENTITY(1,1),
    CreatorId          INT      NOT NULL REFERENCES Users(UserId),
    RetadorId          INT      NULL     REFERENCES Users(UserId),
    WordId             INT      NOT NULL REFERENCES Words(WordId),
    Status             INT      NOT NULL DEFAULT 0,  -- 0=Waiting 1=InProgress 2=Finished 3=Abandoned
    Description        NVARCHAR(500) NULL,
    CreatedAt          DATETIME NOT NULL DEFAULT GETDATE(),
    StartedAt          DATETIME NULL,
    FinishedAt         DATETIME NULL,
    WinnerId           INT      NULL     REFERENCES Users(UserId),
    AbandonedByUserId  INT      NULL     REFERENCES Users(UserId)
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GameMoves')
CREATE TABLE GameMoves (
    MoveId    INT    PRIMARY KEY IDENTITY(1,1),
    GameId    INT    NOT NULL REFERENCES Games(GameId),
    UserId    INT    NOT NULL REFERENCES Users(UserId),
    Letter    NCHAR(1) NOT NULL,
    IsCorrect BIT    NOT NULL,
    MoveDate  DATETIME NOT NULL DEFAULT GETDATE()
);

-- ============================================================
-- Seed Categories (ES)
-- ============================================================
IF NOT EXISTS (SELECT * FROM Categories WHERE Name = 'Animales')
BEGIN
    INSERT INTO Categories (Name, LanguageCode) VALUES
        ('Animales',    'ES'),
        ('Paises',      'ES'),
        ('Ciencias',    'ES'),
        ('Deportes',    'ES'),
        ('Tecnologia',  'ES'),
        ('General',     'ES'),
        ('Animals',     'EN'),
        ('Countries',   'EN'),
        ('Science',     'EN'),
        ('Sports',      'EN'),
        ('Technology',  'EN'),
        ('General',     'EN');
END;

-- ============================================================
-- Seed Words
-- ============================================================
IF NOT EXISTS (SELECT * FROM Words WHERE CategoryId = 1)
BEGIN
    -- Animales / ES / Facil
    INSERT INTO Words (CategoryId, Text, Hint, LanguageCode, Difficulty) VALUES
        (1,'ELEFANTE',   'El animal terrestre mas grande del mundo',    'ES', 1),
        (1,'JIRAFA',     'Tiene el cuello mas largo de todos los animales','ES',1),
        (1,'DELFIN',     'Mamifero marino muy inteligente',             'ES', 1),
        (1,'MARIPOSA',   'Insecto con alas de colores',                 'ES', 1),
        (1,'COCODRILO',  'Reptil de piel dura que vive en rios',        'ES', 2),
        (1,'MURCIELAGO', 'Mamifero volador nocturno',                   'ES', 2),
        (1,'CAMALEÓN',   'Reptil que cambia de color',                  'ES', 3),
        -- Paises / ES
        (2,'ARGENTINA',  'Pais del tango y el mate',                   'ES', 1),
        (2,'BRASIL',     'Pais del carnaval y el futbol',               'ES', 1),
        (2,'MEXICO',     'Pais con piramides aztecas',                  'ES', 1),
        (2,'COLOMBIA',   'Pais del cafe de calidad mundial',            'ES', 1),
        (2,'KAZAJISTAN', 'El pais sin litoral mas grande del mundo',    'ES', 3),
        -- Ciencias / ES
        (3,'ATOMO',      'La unidad basica de la materia',              'ES', 1),
        (3,'NEUTRON',    'Particula sin carga en el nucleo atomico',    'ES', 2),
        (3,'FOTOSINTESIS','Proceso por el que las plantas fabrican alimento','ES',3),
        -- Deportes / ES
        (4,'FUTBOL',     'Deporte mas popular del mundo',               'ES', 1),
        (4,'BALONCESTO', 'Deporte donde se anota metiendo un balon en la canasta','ES',2),
        (4,'ESGRIMA',    'Deporte de combate con espada',               'ES', 2),
        -- Tecnologia / ES
        (5,'COMPUTADORA','Maquina electronica que procesa informacion', 'ES', 1),
        (5,'ALGORITMO',  'Conjunto de pasos para resolver un problema', 'ES', 2),
        (5,'CRIPTOGRAFIA','Tecnica para cifrar informacion',            'ES', 3),
        -- General / ES
        (6,'HANGMAN',    'El juego que estas jugando ahora mismo',      'ES', 1),
        (6,'VOCABULARIO','Conjunto de palabras de un idioma',           'ES', 2),
        -- Animals / EN
        (7,'ELEPHANT',   'The largest land animal on Earth',            'EN', 1),
        (7,'GIRAFFE',    'Has the longest neck of all animals',         'EN', 1),
        -- Countries / EN
        (8,'ARGENTINA',  'Country of tango and mate',                   'EN', 1),
        (8,'BRAZIL',     'Country of carnival and football',            'EN', 1);
END;
