select ops."ExecId", ops."Operation"->'Name'
from (select "Id" as "ExecId", jsonb_array_elements("OriginalGraph"->'Operations') as "Operation"
      from public."Sessions") as ops
