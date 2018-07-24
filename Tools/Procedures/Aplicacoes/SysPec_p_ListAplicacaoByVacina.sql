declare @o varchar(200); set @o = 'SysPec_p_ListAplicacaoByVacina';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_ListAplicacaoByVacina(
	@VacinaId int,
	@FazendaId int
) 
with encryption as
begin

	select	
		Id,
		Vacina,
		Metodo,
		CriadoEm,
		Anotacoes,
		Validade,
		Dosagem,
		Animal,
		CodigoAnimal,
		Lote,
		LoteNome,
		FazendaAnimal
	from
		SysPec_c_Aplicacoes [Aplicacao]
	where 
		Vacina = @VacinaId and FazendaAnimal = @FazendaId

	for xml auto, elements, root('Aplicacoes');

end;
go