namespace RfoLogViewer.Data
{
    /// <summary>
    /// SQL fragments derived from PB w_log.xml and dw_log_table.xml.
    /// </summary>
    internal static class LogQueries
    {
        public const string GetUserId =
            @"SELECT user_id FROM cd_users WHERE UPPER(user_name) = UPPER(:login)";

        public const string OpenContext =
            @"BEGIN pack_context.contextid_open(:contextId, :userId, :module); END;";

        public const string RootLogKeys =
            @"SELECT root_log_key || ' (' || TO_CHAR(COUNT(*)) || ' sessions)' AS label,
                     root_log_key,
                     MAX(start_timestamp) AS max_start,
                     CASE WHEN MAX(NVL(total_errors, 0)) > 0 THEN 'E'
                          WHEN MAX(NVL(total_warnings, 0)) > 0 THEN 'W'
                          ELSE 'N' END AS node_status
              FROM log_struct
              WHERE parent_log_struct_id IS NULL
                AND start_timestamp >= :beginDate
                AND start_timestamp < :endDate
                AND (context_id = pack_install.get_context_id() OR context_id IS NULL)
              GROUP BY root_log_key
              ORDER BY max_start DESC";

        public const string PeriodStatus =
            @"SELECT CASE WHEN MAX(NVL(total_errors, 0)) > 0 THEN 'E'
                         WHEN MAX(NVL(total_warnings, 0)) > 0 THEN 'W'
                         ELSE 'N' END AS node_status
              FROM log_struct
              WHERE parent_log_struct_id IS NULL
                AND start_timestamp >= :beginDate
                AND start_timestamp < :endDate
                AND (context_id = pack_install.get_context_id() OR context_id IS NULL)";

        public const string OrphanStatus =
            @"SELECT CASE WHEN SUM(CASE WHEN log_type = 'E' THEN 1 ELSE 0 END) > 0 THEN 'E'
                         WHEN SUM(CASE WHEN log_type = 'W' THEN 1 ELSE 0 END) > 0 THEN 'W'
                         ELSE 'N' END AS node_status
              FROM log_table
              WHERE root_log_key IS NULL
                AND datetime >= :beginDate
                AND datetime < :endDate
                AND log_type <> 'P'";

        public const string LogSessionStatus =
            @"SELECT NVL(total_errors, 0) AS total_errors,
                     NVL(total_warnings, 0) AS total_warnings
              FROM log_struct
              WHERE log_struct_id = :logStructId";

        public const string RootLogSessions =
            @"SELECT *
              FROM (
                SELECT pack_log.tree_label(l.rowid) AS label,
                       l.log_struct_id,
                       l.parent_log_struct_id,
                       ROUND(86400 * (
                         TO_DATE(TO_CHAR(l.end_timestamp, 'YYYYMMDDHH24MISS'), 'YYYYMMDDHH24MISS')
                         - TO_DATE(TO_CHAR(l.start_timestamp, 'YYYYMMDDHH24MISS'), 'YYYYMMDDHH24MISS')
                       )) AS duration_seconds,
                       CASE WHEN l.end_timestamp IS NULL THEN
                         CASE WHEN l.session_id IN (
                           SELECT pack_install.compute_fermat_sid(sid, serial#, inst_id)
                           FROM gv$session
                         ) THEN 6 ELSE 8 END
                       WHEN NVL(l.total_errors, 0) > 0 THEN 5
                       WHEN NVL(l.total_warnings, 0) > 0 THEN 7
                       ELSE 4 END AS picture_index,
                       NVL(l.total_errors, 0) AS total_errors,
                       NVL(l.total_warnings, 0) AS total_warnings,
                       CASE WHEN (
                         SELECT COUNT(*) FROM log_struct ls2
                         WHERE ls2.parent_log_struct_id = l.log_struct_id
                       ) = 0 THEN 'N' ELSE 'Y' END AS has_children
                FROM log_struct l
                WHERE l.parent_log_struct_id IS NULL
                  AND l.start_timestamp >= :beginDate
                  AND l.start_timestamp < :endDate
                  AND (l.context_id = pack_install.get_context_id() OR l.context_id IS NULL)
                  AND l.root_log_key = :rootLogKey
                ORDER BY l.log_struct_id DESC
              )
              WHERE ROWNUM <= 1000";

        public const string ChildLogSessions =
            @"SELECT *
              FROM (
                SELECT pack_log.tree_label(l.rowid, :rootDuration) AS label,
                       l.log_struct_id,
                       l.parent_log_struct_id,
                       :rootDuration AS duration_seconds,
                       CASE WHEN l.end_timestamp IS NULL THEN
                         CASE WHEN l.session_id IN (
                           SELECT pack_install.compute_fermat_sid(sid, serial#, inst_id)
                           FROM gv$session
                         ) THEN 6 ELSE 8 END
                       WHEN NVL(l.total_errors, 0) > 0 THEN 5
                       WHEN NVL(l.total_warnings, 0) > 0 THEN 7
                       ELSE 4 END AS picture_index,
                       NVL(l.total_errors, 0) AS total_errors,
                       NVL(l.total_warnings, 0) AS total_warnings,
                       CASE WHEN (
                         SELECT COUNT(*) FROM log_struct ls2
                         WHERE ls2.parent_log_struct_id = l.log_struct_id
                       ) = 0 THEN 'N' ELSE 'Y' END AS has_children
                FROM log_struct l
                WHERE l.parent_log_struct_id = :parentLogStructId
                ORDER BY l.log_struct_id ASC
              )
              WHERE ROWNUM <= 1000";

        public const string LogStructGrid =
            @"SELECT log_struct_id, start_timestamp, end_timestamp, session_id, depth,
                     log_key, root_log_key, task_id, root_task_id, process_desc, machine,
                     nb_errors, nb_warnings, nb_infos, total_errors, total_warnings, context_id
              FROM log_struct
              WHERE {0}
              ORDER BY log_struct_id";

        public const string LogTableMain =
            @"SELECT lt.session_id,
                     lt.log_id,
                     lt.log_key,
                     lt.log_type,
                     lt.severity,
                     lt.tech_func,
                     lt.depth,
                     lt.datetime,
                     pack_product.product_name(lt.application) AS product_name,
                     lt.function,
                     lt.step,
                     lt.message,
                     lt.parameters,
                     lt.nummsg,
                     lt.process_desc,
                     lt.task_id,
                     DECODE(lt.session_id, my_session_id, 'Y', 'N') AS is_my_session_id,
                     lt.root_task_id,
                     lt.machine,
                     CASE WHEN lt.task_id <> lt.root_task_id THEN 'Y' ELSE 'N' END AS cooperator,
                     lt.log_struct_id,
                     lt.root_log_key,
                     lt.partition_key
              FROM log_table lt,
                   (SELECT pack_install.get_fermat_sid() AS my_session_id FROM dual)
              WHERE {0}
              ORDER BY lt.log_id";

        public const string UpdateLogSessionLabel =
            @"SELECT pack_log.tree_label(rowid, :rootDuration)
              FROM log_struct
              WHERE log_struct_id = :logStructId";

        public const string GetLogQuery =
            @"SELECT query FROM fdw_log_query WHERE log_id = :logId";
    }
}
