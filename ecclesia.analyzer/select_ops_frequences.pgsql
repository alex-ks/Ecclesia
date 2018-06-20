select ops."UserId", ops."Operation"->'Name' as "OpName", count(ops."Operation"->'Name')
from (select "Id" as "ExecId", "UserId", jsonb_array_elements("OriginalGraph"->'Operations') as "Operation"
      from public."Sessions") as ops
group by ops."UserId", ops."Operation"->'Name'
