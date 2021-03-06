declare @o varchar(200); set @o = 'SysPec_p_ListAlimentacaoByPasto';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_ListAlimentacaoByPasto(
	@PastoId int
) 
with encryption as
begin

	select
		Alimentacao.*
	from
		SysPec_c_Alimentacoes [Alimentacao]
	where 
		Pasto = @PastoId

	for xml auto, elements, root('Alimentacoes');

end;
go