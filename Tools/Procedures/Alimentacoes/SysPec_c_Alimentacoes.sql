declare @o varchar(200); set @o = 'SysPec_c_Alimentacoes';
if object_id(@o, 'V') is not null begin
	declare @d nvarchar(250); set @d = 'drop view ' + @o;
	execute sp_executesql @d;
end;
go
create view SysPec_c_Alimentacoes with encryption as
	select
		Ap.Id,
		Ap.Animal,
		Ap.Racao,
		Ap.CriadoEm,
		Ap.Pasto,
		Ap.Anotacoes,
		Ap.Peso,
		Ap.Antigo,
		An.Codigo as [CodigoAnimal],
		An.Lote,
		An.LoteNome,
		An.Fazenda as [FazendaAnimal],
		Pa.Nome as [PastoNome]
	from
		Alimentacoes Ap
		inner join SysPec_c_Animais An on An.Id = Ap.Animal
		inner join Pastos Pa on Ap.Pasto = Pa.Id
GO