
/* full text search */
/* ---------------- */

sp_fulltext_database 'enable'
go
create fulltext catalog ftOrbium -- in path 'x:\...'
go
create fulltext index on orb_customers (cst_name)
            key index cst_Customer on ftOrbium
            with change_tracking auto
go
create fulltext index on orb_articles (art_name, art_summary, art_text)
            key index art_Article on ftOrbium
            with change_tracking auto
go
--create fulltext index on orb_cases (cas_title, cas_description)
--            key index cas_Case on ftOrbium
--            with change_tracking auto
--go
create fulltext index on orb_emailtemplates (emt_name, emt_subject, emt_body)
            key index emt_EmailTemplate on ftOrbium
            with change_tracking auto
go
create fulltext index on orb_companies (com_name)
            key index com_Company on ftOrbium
            with change_tracking auto
go
create fulltext index on orb_faxtemplates (fxt_name, fxt_subject, fxt_covertext, fxt_body)
            key index fxt_FaxTemplate on ftOrbium
            with change_tracking auto
go
create fulltext index on orb_lettertemplates (lte_name, lte_subject, lte_body)
            key index lte_LetterTemplate on ftOrbium
            with change_tracking auto
go
create fulltext index on orb_RelationEmails (ree_Subject, ree_Body)
            key index pk_RelationEmails on ftOrbium
            with change_tracking auto
go
create fulltext index on orb_QueueInEmails (qie_Subject, qie_Body)
            key index qie_queueinemail on ftOrbium
            with change_tracking auto
go
create fulltext index on orb_SmsTemplates (smt_name, smt_body)
            key index smt_SmsTemplate on ftOrbium
            with change_tracking auto
go

/* script para checar tabelas que utilizar o recurso do fulltext */
select 
	OBJECT_NAME(c2.object_id) [table] 
	,c2.name
	,c2.column_id
	from sys.fulltext_index_columns c1 
	inner join sys.columns c2 
		on c1.object_id = c2.object_id and c1.column_id = c2.column_id



/* snapshot isolation */
/* ------------------ */

declare @c nvarchar(4000)
set @c = 'alter database ' + db_name() + ' set allow_snapshot_isolation on'
exec sp_executesql @c
go

/* initial data */
/* ------------ */

insert into orb_DayHours (dah_hour) values ('00')
insert into orb_DayHours (dah_hour) values ('01')
insert into orb_DayHours (dah_hour) values ('02')
insert into orb_DayHours (dah_hour) values ('03')
insert into orb_DayHours (dah_hour) values ('04')
insert into orb_DayHours (dah_hour) values ('05')
insert into orb_DayHours (dah_hour) values ('06')
insert into orb_DayHours (dah_hour) values ('07')
insert into orb_DayHours (dah_hour) values ('08')
insert into orb_DayHours (dah_hour) values ('09')
insert into orb_DayHours (dah_hour) values ('10')
insert into orb_DayHours (dah_hour) values ('11')
insert into orb_DayHours (dah_hour) values ('12')
insert into orb_DayHours (dah_hour) values ('13')
insert into orb_DayHours (dah_hour) values ('14')
insert into orb_DayHours (dah_hour) values ('15')
insert into orb_DayHours (dah_hour) values ('16')
insert into orb_DayHours (dah_hour) values ('17')
insert into orb_DayHours (dah_hour) values ('18')
insert into orb_DayHours (dah_hour) values ('19')
insert into orb_DayHours (dah_hour) values ('20')
insert into orb_DayHours (dah_hour) values ('21')
insert into orb_DayHours (dah_hour) values ('22')
insert into orb_DayHours (dah_hour) values ('23')
go

/* Set value for Calendar Control */
/* ---- */
update orb_CalendarExclusions
	set cex_DayMonth = convert(char(6), cex_Day , 101)
go

/* Set Default Value */
/* ---- */
update orb_articles
	set art_hiddenLogin = 0
go

/* Set Default Value */
/* ---- */
update orb_operations 
	set ope_AddressLocationConstraint = 0
go

/* Set Default Value */
/* ---- */
update orb_EmailRules 
	set eru_OperatorCondition = 0
where eru_OperatorCondition is null
go


print 'Criando linhas na orb_AnalysisControl...';
insert into orb_AnalysisControl values (0x, 0x, 0x, 0x);
go

print 'Criando linhas na orb_DurationRangeDim...';
insert into orb_DurationRangeDim values (0, ' 0'' até  5''', 0, 5);
insert into orb_DurationRangeDim values (1, ' 5'' até 10''', 5, 10);
insert into orb_DurationRangeDim values (2, '10'' até 15''', 10, 15);
insert into orb_DurationRangeDim values (3, '15'' até 20''', 15, 20);
insert into orb_DurationRangeDim values (4, '20'' até 25''', 20, 25);
insert into orb_DurationRangeDim values (5, '25'' até 30''', 25, 30);
insert into orb_DurationRangeDim values (6, 'acima de 30''', 30, 1000000);
go

print 'Criando linhas na orb_TimeDim...';
exec orb_p_AddTimeDim N'2010-01-01'
go

print 'Adicionando coluna computada a tabela orb_FormInputs'
alter table orb_FormInputs
	add fri_Number_C as (
		case isnumeric(fri_Number)
			when 1 then cast(fri_Number as float)
			else 99999
		end
	) PERSISTED
go

/* Script somente para migração da orb_Calendars e orb_CalendarExclusions depois da execução do dbcheck*/
print 'orb_CalendarExclusions - Atualização da tabela orb_CalendarExclusions'

update orb_CalendarExclusions 
	set cex_DayMonth = case cex_Fixed
							when 1 then convert(char(6), cex_DayMonth , 101) 
							when 0 then  convert(char(10),cex_DayMonth, 101) 
					   end;
go


print 'orb_Calendars - Atualização da tabela orb_Calendars'
update orb_Calendars 
		set cld_Minutes1 = (len(replace(cld_Hours1,'0','')) * 30)
		,cld_Minutes2 = (len(replace(cld_Hours2,'0','')) * 30)
		,cld_Minutes3 = (len(replace(cld_Hours3,'0','')) * 30)
		,cld_Minutes4 = (len(replace(cld_Hours4,'0','')) * 30)
		,cld_Minutes5 = (len(replace(cld_Hours5,'0','')) * 30)
		,cld_Minutes6 = (len(replace(cld_Hours6,'0','')) * 30)
		,cld_Minutes7 = (len(replace(cld_Hours7,'0','')) * 30)
go
