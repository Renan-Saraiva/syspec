declare @o varchar(200); set @o = 'SysPec_p_ListFazenda';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_ListFazenda(
	@criador int
) 
with encryption as
begin

	select
		Id,
		Nome,
		Criador,
		Abreviatura
	from
		Fazendas [Fazenda]
	where 
		Criador = @criador
	for xml auto, elements, root('Fazendas');

end;
go
