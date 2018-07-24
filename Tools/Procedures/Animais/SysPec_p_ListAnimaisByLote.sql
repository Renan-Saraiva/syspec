declare @o varchar(200); set @o = 'SysPec_p_ListAnimaisByLote';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_ListAnimaisByLote(
	@IdLote int
) 
with encryption as
begin

	select
		Animal.*
	from
		SysPec_c_Animais [Animal]
	where 
		Lote = @IdLote
	for xml auto, elements, root('Animais');

end;
go