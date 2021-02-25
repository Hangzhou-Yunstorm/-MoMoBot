export default {
    'GET /api/users': (req, res) => {
        setTimeout(() => {
            // const { pageSize = 10, pageIndex = 1 } = req.body;
            let { pageSize = 10, pageIndex = 1 } = req.query;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;

            const offset = (pageIndex - 1) * pageSize;
            const max = pageIndex * pageSize;

            let data = [];
            let total = 120;

            for (var i = offset; i < max; i++) {
                data.push({
                    id: i,
                    username: `user ${i}`,
                    email: `${i}0000@together.com`,
                    state: i%2
                })
            }
            res.send({
                data,
                total
            })
        }, 2000)
    }
}