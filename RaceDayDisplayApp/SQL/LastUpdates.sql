drop table [dbo].[LastUpdates]

create TABLE [dbo].[LastUpdates] (
    [MeetingId]			INT            NOT NULL,
    [RaceNumber]		SMALLINT       NOT NULL,
    [UpdateDateTime]    DATETIME       NOT NULL,
    [Refreshinterval] INT      NOT NULL,
	PRIMARY KEY CLUSTERED ([MeetingId] ASC, [RaceNumber] ASC)
);


drop PROCEDURE SetUpdateTime
drop type RaceUpdateType

create TYPE [dbo].[RaceUpdateType] AS TABLE (
    [MeetingId]  INT      NULL,
    [RaceNumber] SMALLINT NULL,
	[Refreshinterval] INT NULL);


create PROCEDURE SetUpdateTime
(
@races RaceUpdateType readonly 
)
AS
declare cur cursor for 
select MeetingId,RaceNumber,Refreshinterval from @races
open cur
declare @meetingId int
declare @raceNumber smallint
declare @refreshinterval int
fetch next from cur into @meetingId,@racenumber,@refreshinterval
while (@@FETCH_STATUS = 0)
begin

	merge LastUpdates as target
	using (values (getdate(), @refreshinterval))
		as source (UpdateDateTime, Refreshinterval)
		on target.MeetingId = @meetingId and target.RaceNumber = @raceNumber
	when matched then
		update
		set UpdateDateTime = source.UpdateDateTime,
		    Refreshinterval = source.Refreshinterval
	when not matched then
		insert ( MeetingId, RaceNumber, UpdateDateTime, Refreshinterval)
		values ( @meetingId, @raceNumber, source.UpdateDateTime, source.Refreshinterval);

    fetch next from cur into @meetingId,@racenumber,@refreshinterval
end
close cur
deallocate cur


DECLARE @TVP RaceUpdateType
INSERT INTO @TVP
VALUES
    (699824,1, -1)
exec SetUpdateTime @TVP

delete from LastUpdates


select getdate() as Now, UpdateDateTime, DATEDIFF(second, UpdateDateTime, getdate()) as Diff
from LastUpdates
where MeetingId = 700440 and RaceNumber=1
