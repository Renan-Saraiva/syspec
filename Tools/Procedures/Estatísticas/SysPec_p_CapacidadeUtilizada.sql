declare @o varchar(200); set @o = 'SysPec_p_CapacidadeUtilizada';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_CapacidadeUtilizada(
	@criador int
)
with encryption as
begin
	Select 
		[F].Id [FazendId],
		[F].Nome [FazendaNome],
		ISNULL([S].QtdAnimaisSuporte,0) [QtdAnimaisSuporte],
		ISNULL([Q].QtdAnimais,0) [QtdAnimais]
	from 
		Fazendas [F]
		left join 
		(
			Select
				SUM(QtdAnimaisSuporte) [QtdAnimaisSuporte],
				F.Id [FazendaId]
			from 
				Fazendas [F]
				inner join Pastos [P] on [P].Fazenda = [F].Id
			where
				[F].Criador = @criador
			group by
				[F].Id
		) as [S] on [F].Id = [S].FazendaId
		left join 
		(
			Select 
				COUNT([A].Id) [QtdAnimais],
				F.Id [FazendaId]
			from 
				Fazendas [F]
				inner join Lotes [L] on [L].Fazenda = [F].Id
				inner join SysPec_c_Animais [A] on [A].Lote = [L].Id
			where 
				[F].Criador = @criador
			group by
				[F].Id
		) as [Q] on [F].Id = [Q].FazendaId	
	where
		[F].Criador = @criador
	for xml raw('CapacidadeUtilizada'), elements, root('Capacidades');
end;
go