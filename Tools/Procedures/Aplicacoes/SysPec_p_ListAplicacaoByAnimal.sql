declare @o varchar(200); set @o = 'SysPec_p_ListAplicacaoByAnimal';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_ListAplicacaoByAnimal(
	@AnimalId int
) 
with encryption as
begin

	select
		Aplicacao.*
	from
		SysPec_c_Aplicacoes [Aplicacao]
	where 
		Animal = @AnimalId

	for xml auto, elements, root('Aplicacoes');

end;
go