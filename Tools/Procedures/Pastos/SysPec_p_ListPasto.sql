declare @o varchar(200); set @o = 'SysPec_p_ListPasto';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_ListPasto(
	@IdFazenda int
) 
with encryption as
begin

	select
		Id,
		Nome,
		QtdAnimaisSuporte,
		Fazenda,
		Anotacoes,
		Habilitado
	from
		Pastos [Pasto]
	where 
		Fazenda = @IdFazenda
	for xml auto, elements, root('Pastos');

end;
go
