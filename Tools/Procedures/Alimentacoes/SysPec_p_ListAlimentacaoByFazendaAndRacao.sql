declare @o varchar(200); set @o = 'SysPec_p_ListAlimentacaoByFazendaAndRacao';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_ListAlimentacaoByFazendaAndRacao(
	@RacaoId int,
	@FazendaId int
) 
with encryption as
begin

	select
		Alimentacao.*
	from
		SysPec_c_Alimentacoes [Alimentacao]
	where 
		Racao = @RacaoId and
		FazendaAnimal = @FazendaId
	
	for xml auto, elements, root('Alimentacoes');

end;
go