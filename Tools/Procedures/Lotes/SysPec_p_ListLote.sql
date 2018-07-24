declare @o varchar(200); set @o = 'SysPec_p_ListLote';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_ListLote(
	@IdFazenda int
) 
with encryption as
begin

	select
		Id,
		Nome,
		Fazenda,
		Anotacoes,
		CriadoEm,
		Habilitado
	from
		Lotes [Lote]
	where 
		Fazenda = @IdFazenda
	for xml auto, elements, root('Lotes');

end;
go
