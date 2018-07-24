declare @o varchar(200); set @o = 'SysPec_c_Animais';
if object_id(@o, 'V') is not null begin
	declare @d nvarchar(250); set @d = 'drop view ' + @o;
	execute sp_executesql @d;
end;
go
create view SysPec_c_Animais with encryption as
	select
		A.Id,
		A.Codigo,
		A.Lote,
		A.NascidoEm,
		A.Habilitado,
		A.Raca,
		A.Sexo,
		A.Peso,
		ISNULL(Nome ,'') [LoteNome],
		L.Fazenda
	from
		Animais as A
		inner join Lotes as L on A.Lote = L.Id
	where
		A.Habilitado = 1
GO