declare @o varchar(200); set @o = 'SysPec_p_AddAplicacaoEmLote';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_AddAplicacaoEmLote(
	@xml xml
) with encryption as
begin

	begin try

		insert into Aplicacoes
		(
			Vacina,
			Metodo,
			CriadoEm,
			Anotacoes,
			Validade,
			Dosagem,
			Animal
		)
		select
			x.n.value('Vacina[1]', 'int'),
			x.n.value('Metodo[1]', 'int'),
			GETDATE(),
			x.n.value('Anotacoes[1]', 'varchar(max)'),
			x.n.value('Validade[1]', 'datetime'),
			x.n.value('Dosagem[1]', 'float'),
			A.Id
		from
			@xml.nodes('/*[1]') x(n)
			inner join SysPec_c_Animais A on A.Lote = x.n.value('Lote[1]', 'int')

	end try
	begin catch
		--execute orb_p_RaiseError;
	end catch;

end;
go