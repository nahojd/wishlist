﻿ALTER TABLE [dbo].[Wish]
    ADD CONSTRAINT [FK_Wish_User] FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[User] ([UserId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

