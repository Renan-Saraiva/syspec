declare @o varchar(200); set @o = 'SysPec_p_AddAplicacao';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_AddAplicacao(
	@xml xml,
	@result int output
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
			x.n.value('Animal[1]', 'int')
		from
			@xml.nodes('/*[1]') x(n);

		set @result = scope_identity();

	end try
	begin catch
		--execute orb_p_RaiseError;
	end catch;

end;
go