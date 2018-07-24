declare @o varchar(200); set @o = 'SysPec_p_ListVacina';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_ListVacina
with encryption as
begin

	select
		Id,
		Nome,
		Anotacoes,
		Criador
	from
		Vacinas [Vacina]
	for xml auto, elements, root('Vacinas');

end;
go