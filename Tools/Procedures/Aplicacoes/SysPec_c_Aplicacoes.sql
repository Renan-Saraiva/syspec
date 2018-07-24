declare @o varchar(200); set @o = 'SysPec_c_Aplicacoes';
if object_id(@o, 'V') is not null begin
	declare @d nvarchar(250); set @d = 'drop view ' + @o;
	execute sp_executesql @d;
end;
go
create view SysPec_c_Aplicacoes with encryption as
	select
		[Aplicacao].Id [Id],
		[Aplicacao].Vacina [Vacina],
		[Aplicacao].Metodo [Metodo],
		[Aplicacao].CriadoEm [CriadoEm],
		[Aplicacao].Anotacoes [Anotacoes],
		[Aplicacao].Validade [Validade],
		[Aplicacao].Dosagem [Dosagem],
		[Aplicacao].Animal [Animal],
		[Animal].Codigo [CodigoAnimal],
		[Animal].Lote [Lote],
		[Animal].LoteNome [LoteNome],
		[Animal].Fazenda [FazendaAnimal]
	from
		Aplicacoes [Aplicacao]
		inner join SysPec_c_Animais [Animal] on [Aplicacao].Animal = [Animal].Id
GO