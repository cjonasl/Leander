--Ran successfully on prod 2019-09-03 kl. 17:10
ALTER TABLE dbo.JobStatistics ADD
StatTrack10 datetime NULL,
StatTrack20 datetime NULL,
StatTrack30 datetime NULL,
StatTrack40 datetime NULL,
StatTrack50 datetime NULL,
StatTrack60 datetime NULL,
StatTrack70 datetime NULL,
StatTrack80 datetime NULL,
StatJobClosed datetime NULL,
StatJobClosedUserID varchar(10) NULL,
StatJobCompleted datetime NULL,
StatJobCompletedUserID varchar(10) NULL,
StatJobStatus smallint NULL,
StatClaimSentDate datetime NULL,
StatSalesInvoiceCount int NULL,
StatCurrentMood smallint NULL,
StatOverallMood smallint NULL,
StatTelephoneCount int NULL,
StatTelephoneMinutes int NULL,
StatFirstVisitDate date NULL,
StatSecondVisitDate date NULL,
StatThirdVisitDate date NULL,
StatFourthVisitDate date NULL,
StatQuotedMinutes int NULL,
StatBookedMinutes int NULL,
StatChargedMinutes int NULL