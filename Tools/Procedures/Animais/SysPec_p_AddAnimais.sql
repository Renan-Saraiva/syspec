declare @o varchar(200); set @o = 'SysPec_p_AddAnimais';
if object_id(@o, 'P') is not null begin
	declare @d nvarchar(250); set @d = 'drop procedure ' + @o;
	execute sp_executesql @d;
end;
go
create procedure SysPec_p_AddAnimais(
	@xml xml,
	@IdFazenda int,
	@IdLote int
) with encryption as
begin	
	begin try
		if @IdFazenda > 0 and @IdLote > 0
			begin
				insert into 
					Animais 
					(
						Codigo,
						Lote,
						NascidoEm,
						Raca,
						Sexo,
						Peso
					)
				select
					null,
					@IdLote,
					x.n.value('NascidoEm[1]', 'datetime'),
					x.n.value('Raca[1]', 'int'),
					x.n.value('Sexo[1]', 'int'),
					x.n.value('Peso[1]', 'float')
				from
					@xml.nodes('/Animais/Animal') x(n);


				declare @abreviatura varchar(3);
				select  top 1 @abreviatura =  Abreviatura from Fazendas where Id = @IdFazenda
				
				update Animais 
					set 
						Codigo = @abreviatura + convert(nvarchar(23),Id)
					where 
						Codigo is null and 
						Lote = @IdLote
			end
	end try
	begin catch
		--execute orb_p_RaiseError;
	end catch;

end;
go
