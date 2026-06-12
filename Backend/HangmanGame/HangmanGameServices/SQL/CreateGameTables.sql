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
    Status             INT      NOT NULL DEFAULT 0,  -- 0=Waiting 1=InProgress 2=Finished 3=Abandoned 4=Cancelled
    Description        NVARCHAR(500) NULL,
    CreatedAt          DATETIME NOT NULL DEFAULT GETDATE(),
    StartedAt          DATETIME NULL,
    FinishedAt         DATETIME NULL,
    WinnerId           INT      NULL     REFERENCES Users(UserId),
    AbandonedByUserId  INT      NULL     REFERENCES Users(UserId)
);

IF OBJECT_ID('dbo.CK_Games_Status', 'C') IS NULL
ALTER TABLE Games
ADD CONSTRAINT CK_Games_Status CHECK (Status IN (0, 1, 2, 3, 4));

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

-- ============================================================
-- Seed extra Words (rounds out every category to ~10 words)
-- ============================================================
IF NOT EXISTS (SELECT * FROM Words WHERE CategoryId = 1 AND Text = N'ARAÑA')
BEGIN
    INSERT INTO Words (CategoryId, Text, Hint, LanguageCode, Difficulty) VALUES
        -- Animales / ES
        (1, N'ARAÑA',    N'Animal con ocho patas que teje telarañas',                'ES', 1),
        (1, N'PIRAÑA',   N'Pez de ríos sudamericanos con dientes filosos',           'ES', 2),
        (1, N'TIBURON',  N'Gran depredador marino con aletas y dientes afilados',    'ES', 1),
        -- Paises / ES
        (2, N'ESPAÑA',   N'País europeo conocido por el flamenco y la paella',       'ES', 1),
        (2, N'CHILE',    N'País largo y angosto de América del Sur',                 'ES', 1),
        (2, N'PERU',     N'País de Machu Picchu',                                    'ES', 1),
        (2, N'ALEMANIA', N'País europeo conocido por la cerveza y el fútbol',        'ES', 1),
        (2, N'JAPON',    N'País asiático conocido por el sushi y el monte Fuji',     'ES', 1),
        -- Ciencias / ES
        (3, N'GRAVEDAD',    N'Fuerza que atrae los objetos hacia la Tierra',         'ES', 1),
        (3, N'MOLECULA',    N'Conjunto de átomos unidos químicamente',               'ES', 2),
        (3, N'ELECTRON',    N'Partícula subatómica con carga negativa',              'ES', 2),
        (3, N'GENETICA',    N'Ciencia que estudia la herencia biológica',            'ES', 2),
        (3, N'EVOLUCION',   N'Proceso de cambio de las especies a lo largo del tiempo','ES', 3),
        (3, N'MAGNETISMO',  N'Fenómeno físico que atrae el hierro',                  'ES', 2),
        (3, N'ENERGIA',     N'Capacidad de un sistema para realizar trabajo',        'ES', 1),
        -- Deportes / ES
        (4, N'TENIS',       N'Deporte que se juega con raqueta y pelota sobre una cancha','ES', 1),
        (4, N'NATACION',    N'Deporte que se practica en el agua',                   'ES', 1),
        (4, N'CICLISMO',    N'Deporte que se practica sobre una bicicleta',          'ES', 1),
        (4, N'BOXEO',       N'Deporte de combate con guantes acolchados',            'ES', 1),
        (4, N'VOLEIBOL',    N'Deporte donde se golpea un balón por encima de una red','ES', 2),
        (4, N'MONTAÑISMO',  N'Deporte que consiste en escalar montañas',             'ES', 2),
        (4, N'GIMNASIA',    N'Deporte que combina fuerza, flexibilidad y equilibrio','ES', 2),
        -- Tecnologia / ES
        (5, N'INTERNET',    N'Red global que conecta computadoras en todo el mundo', 'ES', 1),
        (5, N'SOFTWARE',    N'Conjunto de programas que ejecuta una computadora',    'ES', 1),
        (5, N'HARDWARE',    N'Componentes físicos de una computadora',               'ES', 1),
        (5, N'ROBOTICA',    N'Ciencia que diseña y construye robots',                'ES', 2),
        (5, N'BLOCKCHAIN',  N'Tecnología de registro distribuido usada en criptomonedas','ES', 3),
        (5, N'SERVIDOR',    N'Computadora que provee servicios a otras a través de una red','ES', 2),
        (5, N'DISEÑO',      N'Proceso creativo para crear interfaces o productos',   'ES', 1),
        -- General / ES
        (6, N'MONTAÑA',     N'Gran elevación natural del terreno',                   'ES', 1),
        (6, N'PELICULA',    N'Obra audiovisual que se proyecta en el cine',          'ES', 1),
        (6, N'MUSICA',      N'Arte de combinar sonidos de manera armoniosa',         'ES', 1),
        (6, N'PINTURA',     N'Arte de representar imágenes usando colores',          'ES', 1),
        (6, N'ESCRITURA',   N'Sistema para representar el lenguaje mediante símbolos','ES', 2),
        (6, N'AVENTURA',    N'Suceso emocionante o fuera de lo común',               'ES', 1),
        (6, N'MISTERIO',    N'Algo que no tiene explicación conocida',               'ES', 2),
        (6, N'SORPRESA',    N'Algo inesperado que causa asombro',                    'ES', 1),
        -- Animals / EN
        (7, N'DOLPHIN',     N'Intelligent marine mammal known for its playful behavior','EN', 1),
        (7, N'BUTTERFLY',   N'Insect with colorful wings',                           'EN', 1),
        (7, N'CROCODILE',   N'Reptile with tough skin that lives in rivers',         'EN', 2),
        (7, N'BAT',         N'Nocturnal flying mammal',                              'EN', 2),
        (7, N'CHAMELEON',   N'Reptile that changes color',                           'EN', 3),
        (7, N'SPIDER',      N'Eight-legged creature that spins webs',                'EN', 1),
        (7, N'PIRANHA',     N'South American river fish with sharp teeth',           'EN', 2),
        (7, N'SHARK',       N'Large marine predator with sharp teeth',               'EN', 1),
        -- Countries / EN
        (8, N'MEXICO',      N'Country with Aztec pyramids',                          'EN', 1),
        (8, N'COLOMBIA',    N'Country known for world-class coffee',                 'EN', 1),
        (8, N'KAZAKHSTAN',  N'The largest landlocked country in the world',          'EN', 3),
        (8, N'SPAIN',       N'European country known for flamenco and paella',       'EN', 1),
        (8, N'CHILE',       N'Long and narrow country in South America',             'EN', 1),
        (8, N'PERU',        N'Country home to Machu Picchu',                         'EN', 1),
        (8, N'GERMANY',     N'European country known for beer and football',         'EN', 1),
        (8, N'JAPAN',       N'Asian country known for sushi and Mount Fuji',         'EN', 1),
        -- Science / EN
        (9, N'ATOM',           N'The basic unit of matter',                         'EN', 1),
        (9, N'NEUTRON',        N'Particle with no charge in the atomic nucleus',     'EN', 2),
        (9, N'PHOTOSYNTHESIS', N'Process by which plants make food',                 'EN', 3),
        (9, N'GRAVITY',        N'Force that pulls objects toward Earth',             'EN', 1),
        (9, N'MOLECULE',       N'Group of atoms bonded together',                    'EN', 2),
        (9, N'ELECTRON',       N'Subatomic particle with a negative charge',         'EN', 2),
        (9, N'GENETICS',       N'The study of heredity',                            'EN', 2),
        (9, N'EVOLUTION',      N'The process of change in species over time',        'EN', 3),
        (9, N'MAGNETISM',      N'Physical phenomenon that attracts iron',            'EN', 2),
        (9, N'ENERGY',         N'The capacity of a system to do work',               'EN', 1),
        -- Sports / EN
        (10, N'SOCCER',         N'The most popular sport in the world',              'EN', 1),
        (10, N'BASKETBALL',     N'Sport where you score by putting a ball through a hoop','EN', 2),
        (10, N'FENCING',        N'Combat sport played with swords',                  'EN', 2),
        (10, N'TENNIS',         N'Sport played with a racket and a ball on a court',  'EN', 1),
        (10, N'SWIMMING',       N'Sport practiced in the water',                      'EN', 1),
        (10, N'CYCLING',        N'Sport practiced on a bicycle',                      'EN', 1),
        (10, N'BOXING',         N'Combat sport with padded gloves',                   'EN', 1),
        (10, N'VOLLEYBALL',     N'Sport where players hit a ball over a net',         'EN', 2),
        (10, N'MOUNTAINEERING', N'Sport of climbing mountains',                       'EN', 3),
        (10, N'GYMNASTICS',     N'Sport combining strength, flexibility and balance', 'EN', 2),
        -- Technology / EN
        (11, N'COMPUTER',     N'Electronic machine that processes information',      'EN', 1),
        (11, N'ALGORITHM',    N'A set of steps to solve a problem',                  'EN', 2),
        (11, N'CRYPTOGRAPHY', N'Technique for encrypting information',               'EN', 3),
        (11, N'INTERNET',     N'Global network that connects computers worldwide',   'EN', 1),
        (11, N'SOFTWARE',     N'Programs that run on a computer',                    'EN', 1),
        (11, N'HARDWARE',     N'Physical components of a computer',                  'EN', 1),
        (11, N'ROBOTICS',     N'Science of designing and building robots',           'EN', 2),
        (11, N'BLOCKCHAIN',   N'Distributed ledger technology used in cryptocurrencies','EN', 3),
        (11, N'SERVER',       N'Computer that provides services to others over a network','EN', 2),
        (11, N'DESIGN',       N'Creative process for creating interfaces or products','EN', 1),
        -- General / EN
        (12, N'HANGMAN',    N'The game you are playing right now',                   'EN', 1),
        (12, N'VOCABULARY', N'The set of words of a language',                       'EN', 2),
        (12, N'MOUNTAIN',   N'Large natural elevation of the terrain',               'EN', 1),
        (12, N'MOVIE',      N'Audiovisual work shown in theaters',                   'EN', 1),
        (12, N'MUSIC',      N'Art of combining sounds in a harmonious way',          'EN', 1),
        (12, N'PAINTING',   N'Art of representing images using colors',              'EN', 1),
        (12, N'WRITING',    N'System for representing language through symbols',     'EN', 2),
        (12, N'ADVENTURE',  N'An exciting or unusual experience',                    'EN', 1),
        (12, N'MYSTERY',    N'Something that has no known explanation',              'EN', 2),
        (12, N'SURPRISE',   N'Something unexpected that causes astonishment',        'EN', 1);
END;
