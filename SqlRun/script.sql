---сказать что при update использовать alias из списка from

UPDATE Es
SET
	RevenueAllocation = I.Rate,
	_ModifiedOn = GetDate(),
	_ModifiedBy = 0
FROM MARS_EngagementServiceSource I 
INNER JOIN [Business].Engagement E ON E.Code = I.EngagementCode AND E.FirmID = I.FirmID
INNER JOIN Business_Service S ON S.Code = I.ServiceCode
INNER JOIN [Business].EngagementService ES ON ES.EngagementID = E.ID  AND ES.ServiceID = S.ID
WHERE ES.RevenueAllocation <> I.Rate

------

--DECLARE @t TABLE ( PermissionCode NVARCHAR(MAX)
--,                  RoleID         int )
--INSERT INTO @t ( PermissionCode,                    RoleID )
--VALUES         ( 'Audit.DPP.ReportingGroup.Member', 20     )
--,              ( 'Audit.DPP.Administrator',         16     )
--
--INSERT INTO Security_FirmUserPermission ( UserID, FirmID, PermissionID, _Container, _CreatedOn, _CreatedBy, _ModifiedOn, _ModifiedBy )
--SELECT sur.UserID
--,      1
--,      sp.ID
--,      'Security.FirmUserPermission'
--,      GETDATE()
--,      0
--,      GETDATE()
--,      0
--, t.t
--FROM      Security_FirmPermission     AS sfp
--JOIN      Security_Permission         AS sp  ON sp.ID = sfp.ID
--JOIN      @t                             t   ON t.PermissionCode = sp.Code
--JOIN      Security_User_Role          AS sur ON t.RoleID=sur.RoleID
--LEFT JOIN Security_FirmUserPermission    sf  ON sf.UserID =sur.UserID AND sf.PermissionID = sp.ID AND sf.FirmID = 1
--WHERE sf.ID IS NULL



---Проверки существования полей


--if Exists(select 1 from sys.columns where object_id = object_id('audit_dpp_DeliverableProcessingRequest','U') and name = 'EngagementID')
--begin
--	ALTER TABLE [audit_dpp_DeliverableProcessingRequest] DROP COLUMN EngagementID
--end
--
--if not Exists(select 1 from sys.columns where object_id = object_id('audit_dpp_DeliverableProcessingTaskProfile','U') and name = 'FirmID')
--begin
--	ALTER TABLE audit_dpp_DeliverableProcessingTaskProfile ADD FirmID INT
--end

--ALTER TABLE [audit_dpp_DeliverableProcessingRequest] DROP COLUMN EngagementID
--ALTER TABLE audit_dpp_DeliverableProcessingTaskProfile ADD FirmID INT


--ALTER PROCEDURE [dbo].[Audit_DPP_Deliverables] ( @login NVARCHAR(60))
--AS
--BEGIN

--	SELECT *
--	FROM      [Business].[Engagement]                    e
--	WHERE ad.DeliverableID IN (SELECT ID
--		FROM [dbo].[audit_dpp_AvailableDeliverable](@login) )
--	ORDER BY e.[OpenDate] DESC
--	,        adt.BalanceSheetDate DESC
--END


--select t.id,t.name from (select id,name from tabl) t1

--select t1.id,t.name 
--from (select id,n as name,tab=n1 from t) t1

--select * from tab order by 1

--select t2.id,t2.name 
--from (
--	select t1.id,t1.name
--	from (select id,n as name,tab=n1 from t) t1
--	) t2

--with t1 as(select id,name from tabl) select t.id,t.name from t1


--create table mytable(id int)
--Go 
--insert mytable(id) values(1)

--create table #table(id int)

--declare @n nvarchar(1);
--Declare @tabl table(id int)

-- Declare @t1 bigint = 111111111111111111111111
-- Declare @t2 float = 5.0;

-- Declare @top int = 1;
-- Declare @t int = (select @top)
-- Declare @t3 int = (select top(@top) 1 from t where ttt = 1)
-- insert #table
-- select *,1, t.ta,@n,(select * from t) from tabl1 t

-- select @n
-- with s as(
-- 	select * from ttt
-- ),s as(
-- 	select * from ttt
-- )
-- select t.id,t1.namename
-- from --@tabl t 
-- 	t1 t 
-- 	--join t on t.id=t1.id
-- --join finance.tetra t1 on t.id = t1.parid and t1.name=t.part and t.i = t1.ii and @n = t1.ii11
-- where t.par is null
-- and t.name = t.reper;

-- t.id Equals t1.parid
-- t1.name Equals t.part
-- t.i Equals t1.ii

---Задачи:
--сказать что должен быть GO
--alter table #ttt add FirmID int
--update t set FirmID = f.ID
--from #ttt t left join Firm f on t.Code2 = f.Code

---ошибки 41 44 не работают напримере:

--if OBJECT_ID('[Mars_EngagementDepStage]') is not null drop table [Mars_EngagementDepStage];
--if OBJECT_ID('[Mars_EngagementDepStage]') is not null drop table [dbo].[Mars_EngagementDepStage];
--if OBJECT_ID('[Mars_EngagementDepStage]') is not null drop table [kdb]..[Mars_EngagementDepStage];
--if OBJECT_ID('[Mars_EngagementDepStage]') is not null drop table [kdb].[dbo].[Mars_EngagementDepStage];

--use [kdb];
--if OBJECT_ID('[dbo].[Mars_EngagementDepStage]') is not null drop table [Mars_EngagementDepStage];
--if OBJECT_ID('[dbo].[Mars_EngagementDepStage]') is not null drop table [dbo].[Mars_EngagementDepStage];
--if OBJECT_ID('[dbo].[Mars_EngagementDepStage]') is not null drop table [kdb]..[Mars_EngagementDepStage];
--if OBJECT_ID('[dbo].[Mars_EngagementDepStage]') is not null drop table [kdb].[dbo].[Mars_EngagementDepStage];

--if OBJECT_ID('[kdb]..[Mars_EngagementDepStage]') is not null drop table [Mars_EngagementDepStage];
--if OBJECT_ID('[kdb]..[Mars_EngagementDepStage]') is not null drop table [dbo].[Mars_EngagementDepStage];
--if OBJECT_ID('[kdb]..[Mars_EngagementDepStage]') is not null drop table [kdb]..[Mars_EngagementDepStage];
--if OBJECT_ID('[kdb]..[Mars_EngagementDepStage]') is not null drop table [kdb].[dbo].[Mars_EngagementDepStage];

--if OBJECT_ID('[kdb].dbo.[Mars_EngagementDepStage]') is not null drop table [Mars_EngagementDepStage];
--if OBJECT_ID('[kdb].dbo.[Mars_EngagementDepStage]') is not null drop table [dbo].[Mars_EngagementDepStage];
--if OBJECT_ID('[kdb].dbo.[Mars_EngagementDepStage]') is not null drop table [kdb]..[Mars_EngagementDepStage];
--if OBJECT_ID('[kdb].dbo.[Mars_EngagementDepStage]') is not null drop table [kdb].[dbo].[Mars_EngagementDepStage];


--go
--CREATE TABLE #entityform_temp(
--	[EngagementCode] [nvarchar](50) NOT NULL,
--	[EmployeeCode] [nvarchar](50) NOT NULL,
--	[DEP] [varchar](10) NOT NULL
--) ON [PRIMARY]

--не говорить о том что при скалярной выборке использовать top если используется 1 агрегатная функция:
--select max(t.IdEntityForm) from #entityform_temp t where [DEP] is not null

---обработка транзакций
--begin tran r
--commit tran r
--rollback tran r

--all update

--create table tab(id1 int)

--update tab set id = 1 -- проверить таблицу и поле id

--update tab set id = 1 -- не верная таблица tab1
--from #tab t

--update t1 set id = 1 -- предложить поставить алиас
--from tab t join tab1 t1 on t.id=t1.id

--update tab set id = 1
--from tab
--where id <> 1

--update tab set id = 1 -- предложить использовать алиас
--from tab t
--where id <> 1

--update tab set id = 1
--from tab t
--where t.id <> 1

--update t set id = 1
--from tab t
--where t.id <> 1

--update t set id = 1        --ошибка, поле не было выбрано
--from (select n from tab) t 
--where rr.id <> 1 -- не верный алиас 

--with s as(
--	select n from tab
--	where id <> 1
--	)
--update s set n = 1

--update t set id = t2.id
--from tab t join tab1 t1 on t.id = t1.id
--where t.id <> 1








