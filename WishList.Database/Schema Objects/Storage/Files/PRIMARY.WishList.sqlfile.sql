ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [WishList], FILENAME = '$(DefaultDataPath)$(DatabaseName).mdf', MAXSIZE = UNLIMITED, FILEGROWTH = 1024 KB) TO FILEGROUP [PRIMARY];

