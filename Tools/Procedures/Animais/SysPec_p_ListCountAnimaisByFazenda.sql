declare @o varchar(200); set @o = 'SysPec_p_ListCountAnimaisByFazenda';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_ListCountAnimaisByFazenda(
	@IdFazenda int,
	@result int output
) 
with encryption as
begin

	select
		@result = count(*)
	from
		SysPec_c_Animais
	where
		Fazenda = @IdFazenda
end;
go