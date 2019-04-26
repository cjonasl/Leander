/*
Email to Vihay and Mark 2019-04-25:

Regarding error store users can not log in.

I think the error regarding that store users can not log in is
solved. The error was that password was not clientID hashed,
probably because error happened when update script was run
(I remember it was a parenthesis missing so the update script
ran only partially).I updated the password now to clientiD
hashed accordnig  to the followiong sql:
*/
UPDATE UserWeb
SET [Password] = HASHBYTES('SHA1', CAST(ClientID AS varchar(10)))
WHERE ClientID IN (SELECT ClientID FROM Client WHERE ClientPriorityBooking = 1)