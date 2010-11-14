ALTER DATABASE [$(DatabaseName)]
    ADD LOG FILE (NAME = [WishList_log], FILENAME = '$(DefaultDataPath)$(DatabaseName)_1.ldf', MAXSIZE = 2097152 MB, FILEGROWTH = 10 %);

