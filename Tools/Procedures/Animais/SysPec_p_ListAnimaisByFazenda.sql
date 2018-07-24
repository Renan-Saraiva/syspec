declare @o varchar(200); set @o = 'SysPec_p_ListAnimaisByFazenda';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_ListAnimaisByFazenda(
	@IdFazenda int
) 
with encryption as
begin

	select
		Animal.*
	from
		SysPec_c_Animais [Animal]
	where
		Fazenda = @IdFazenda
	for xml auto, elements, root('Animais');

end;
go