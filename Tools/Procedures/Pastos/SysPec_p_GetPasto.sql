declare @o varchar(200); set @o = 'SysPec_p_GetPasto';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_GetPasto
(
	@Id int
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
		Id = @Id
	for xml auto, elements;

end;
go
