declare @o varchar(200); set @o = 'SysPec_p_AddAlimentacaoEmLote';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_AddAlimentacaoEmLote(
	@xml xml
) with encryption as
begin
	begin try
		declare @Lote int;

		select
			top 1 @Lote =  x.n.value('Lote[1]', 'int')
		from
			@xml.nodes('/*[1]') x(n)
		
		update Alimentacoes 
			set 
				Antigo = 1 
		where 
			Animal in (select Id from Animais where Lote = @Lote)

		insert into Alimentacoes 
		(
			Animal,
			Racao,
			CriadoEm,
			Pasto,
			Anotacoes,
			Peso
		)
		select
			A.Id,
			x.n.value('Racao[1]', 'int'),
			getdate(),
			x.n.value('Pasto[1]', 'int'),
			x.n.value('Anotacoes[1]', 'varchar(max)'),
			x.n.value('Peso[1]', 'float')
		from
			@xml.nodes('/*[1]') x(n)
			inner join SysPec_c_Animais A on A.Lote = x.n.value('Lote[1]', 'int')

	end try
	begin catch
		--execute orb_p_RaiseError;
	end catch;

end;
go