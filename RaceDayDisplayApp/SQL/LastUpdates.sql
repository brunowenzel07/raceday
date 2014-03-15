drop table [dbo].[LastUpdates]

create TABLE [dbo].[LastUpdates] (
    [MeetingId]			INT            NOT NULL,
    [RaceNumber]		SMALLINT       NOT NULL,
    [UpdateDateTime]    DATETIME       NOT NULL,
	PRIMARY KEY CLUSTERED ([MeetingId] ASC, [RaceNumber] ASC)
);


drop PROCEDURE SetUpdateTime
drop type RaceUpdateType

CREATE TYPE RaceUpdateType AS TABLE 
( MeetingId INT, RaceNumber SMALLINT);

create PROCEDURE SetUpdateTime
(
@races RaceUpdateType readonly 
)
AS
declare cur cursor for 
select MeetingId,RaceNumber from @races
open cur
declare @meetingId int
declare @raceNumber smallint
fetch next from cur into @meetingId,@racenumber
while (@@FETCH_STATUS = 0)
begin

	merge LastUpdates as target
	using (values (getdate()))
		as source (UpdateDateTime)
		on target.MeetingId = @meetingId and target.RaceNumber = @raceNumber
	when matched then
		update
		set UpdateDateTime = source.UpdateDateTime
	when not matched then
		insert ( MeetingId, RaceNumber, UpdateDateTime)
		values ( @meetingId, @raceNumber, source.UpdateDateTime);

    fetch next from cur into @meetingId,@racenumber
end
close cur
deallocate cur


DECLARE @TVP RaceUpdateType
INSERT INTO @TVP
VALUES
    (0412589,1),(0412589,2),(0425896,2),(04789652,1)
exec SetUpdateTime @TVP

delete from LastUpdates


select getdate() as Now, UpdateDateTime, DATEDIFF(second, UpdateDateTime, getdate()) as Diff
from LastUpdates
where MeetingId = 700440 and RaceNumber=1
