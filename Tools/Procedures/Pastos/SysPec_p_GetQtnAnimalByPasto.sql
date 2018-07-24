declare @o varchar(200); set @o = 'SysPec_p_GetQtnAnimalByPasto';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_GetQtnAnimalByPasto
(
	@pasto int,
	@result int output
)
with encryption as
begin

	select 
		@result = count(1)
	from
		SysPec_c_Animais [An]
		inner join Alimentacoes [Al] on [Al].Animal = [An].Id
	where
		Pasto = @pasto and Antigo = 0		
end;
go