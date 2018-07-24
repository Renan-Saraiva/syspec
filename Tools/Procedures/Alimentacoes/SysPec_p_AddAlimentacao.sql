declare @o varchar(200); set @o = 'SysPec_p_AddAlimentacao';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_AddAlimentacao(
	@xml xml,
	@alimentacao int output
) with encryption as
begin

	begin try

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
			x.n.value('Animal[1]', 'int'),
			x.n.value('Racao[1]', 'int'),
			getdate(),
			x.n.value('Pasto[1]', 'int'),
			x.n.value('Anotacoes[1]', 'varchar(max)'),
			x.n.value('Peso[1]', 'float')
		from
			@xml.nodes('/*[1]') x(n);

		set @alimentacao = scope_identity();

	end try
	begin catch
		--execute orb_p_RaiseError;
	end catch;

end;
go