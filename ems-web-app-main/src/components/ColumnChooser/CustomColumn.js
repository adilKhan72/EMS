import React, { Component } from "react";
import Table from "@material-ui/core/Table";
import TableBody from "@material-ui/core/TableBody";
import TableCell from "@material-ui/core/TableCell";
import TableContainer from "@material-ui/core/TableContainer";
import TableHead from "@material-ui/core/TableHead";
import TableRow from "@material-ui/core/TableRow";
import TablePagination from "@material-ui/core/TablePagination";
class CustomColumn extends Component {
  constructor(props) {
    super(props);
    this.state = {
      page: 0,
      rowsPerPage: 10,
    };
  }

  componentDidMount() {
    if (this.props.IsUpdated) {
      this.setState({
        page: this.props.page,
        rowsPerPage: this.props.rowsPerPage,
      });
    }
  }
  handleChangePage = (event, newPage) => {
    this.setState({ page: newPage });
    this.props.handleChangeRows(newPage, this.state.rowsPerPage);
  };
  handleChangeRowsPerPage = (event) => {
    this.setState({ rowsPerPage: parseInt(event.target.value, 10) });
    this.setState({ page: 0 });
  };
  render() {
    return (
      <>
        <TableContainer className="table-responsive" component={"div"}>
          <Table className="table" id="ems_table_vl">
            <TableHead>
              <TableRow>
                {this.props.TableHead.map((item, index) => {
                  return (
                    //   <li value={item.StateName}>
                    //     <Checkbox
                    //       ddAttr={item.StateName}
                    //       onClick={() => {
                    //         this.ColumnsHideSHow(index, item.Status);
                    //       }}
                    //       checked={item.Status}
                    //     />
                    //     {item.ColumnName}
                    //   </li>
                    item.Status == true ? (
                      index == 0 ? (
                        this.props.checkadmin === "SuperAdmin" ||
                        this.props.checkadmin === "Admin" ? (
                          <TableCell>{item.ColumnName}</TableCell>
                        ) : item.StateName == "BillableHours" ? null : (
                          <TableCell>{item.ColumnName}</TableCell>
                        )
                      ) : this.props.checkadmin === "SuperAdmin" ||
                        this.props.checkadmin === "Admin" ? (
                        <TableCell>{item.ColumnName}</TableCell>
                      ) : item.StateName == "BillableHours" ? null : (
                        <TableCell>{item.ColumnName}</TableCell>
                      )
                    ) : // <TableCell className="text-center">
                    //   {item.ColumnName}
                    // </TableCell>
                    null
                  );
                })}
              </TableRow>
            </TableHead>
            <TableBody>
              {this.props.lstReport != null && this.props.lstReport?.length > 0
                ? this.props.lstReport
                    .slice(
                      this.state.page * this.state.rowsPerPage,
                      this.state.page * this.state.rowsPerPage +
                        this.state.rowsPerPage
                    )
                    .map((row, index) => {
                      return (
                        <TableRow
                          style={
                            this.props.checkadmin === "SuperAdmin"
                              ? { cursor: "pointer" }
                              : { cursor: "" }
                          }
                          onClick={() => {
                            if (this.props.checkadmin == "SuperAdmin") {
                              this.props.GetData(row);
                            }
                          }}
                        >
                          {
                            this.props.ModuleName == "Report" ? (
                              row.Rowentry == true ? (
                                this.props.TableHead.map((item, index) => {
                                  if (item.Status == true) {
                                    var valuekey = Object.keys(row).map(
                                      (keyName, i) => {
                                        // var checkcolumn = this.props.TableHead.filter(
                                        //   (SName) =>
                                        //     SName.StateName.trim()
                                        //       .toLowerCase()
                                        //       .includes(keyName.trim().toLowerCase()) &&
                                        //     SName.Status == true
                                        // );
                                        if (
                                          item.StateName.trim().toLowerCase() ==
                                          keyName.trim().toLowerCase()
                                        ) {
                                          return this.props.checkadmin ==
                                            "SuperAdmin" ||
                                            this.props.checkadmin == "Admin"
                                            ? row[keyName]
                                            : keyName.trim().toLowerCase() ==
                                              "BillableHours".toLocaleLowerCase()
                                            ? null
                                            : row[keyName];
                                        }
                                      }
                                    );
                                    return index == 0 ? (
                                      <TableCell>{valuekey}</TableCell>
                                    ) : (
                                      <TableCell>{valuekey}</TableCell>
                                    );
                                  }
                                })
                              ) : (
                                <>
                                  <TableCell
                                    colSpan={5}
                                    className="text-center"
                                    style={{
                                      backgroundColor: "gray",
                                      color: "white",
                                      fontWeight: "bold",
                                      width: "100%",
                                      wordBreak: "break-all",
                                    }}
                                  >
                                    {row.week != null ? row.week : row.Range}
                                  </TableCell>

                                  <TableCell
                                    colSpan={2}
                                    className="text-center"
                                    style={{
                                      backgroundColor: "gray",
                                      color: "white",
                                      fontWeight: "bold",
                                    }}
                                  ></TableCell>
                                  <TableCell
                                    //colSpan={1}
                                    className="text-right"
                                    style={{
                                      backgroundColor: "gray",
                                      color: "white",
                                      fontWeight: "bold",
                                      width: "100%",
                                      wordBreak: "break-all",
                                    }}
                                  >
                                    {row.hours != null
                                      ? "Actual Hrs:" + row.hours
                                      : "Actual Days:" + row.Days}
                                  </TableCell>
                                  <TableCell
                                    //colSpan={1}
                                    className="text-right"
                                    style={{
                                      backgroundColor: "gray",
                                      color: "white",
                                      fontWeight: "bold",
                                    }}
                                  >
                                    {row.Billable_Hours != null
                                      ? "Billable Hrs:" + row.Billable_Hours
                                      : "Billable Days:" + row.Billable_Days}
                                  </TableCell>
                                  <TableCell
                                    //colSpan={1}
                                    className="text-center"
                                    style={{
                                      backgroundColor: "gray",
                                      color: "white",
                                      fontWeight: "bold",
                                    }}
                                  ></TableCell>
                                </>
                              )
                            ) : null
                            // Object.keys(row).map((keyName, i) => {
                            //
                            //   var checkcolumn = this.props.TableHead.filter(
                            //     (SName) =>
                            //       SName.StateName.trim()
                            //         .toLowerCase()
                            //         .includes(keyName.trim().toLowerCase()) &&
                            //       SName.Status == true
                            //   );
                            //   if (checkcolumn.length > 0) {
                            //     return <TableCell>{row[keyName]}</TableCell>;
                            //   }
                            // })
                          }
                        </TableRow>
                      );
                      // <TableRow>
                      //   {Object.keys(row).map((keyName, i) => {
                      //
                      //     var checkcolumn = this.props.TableHead.filter((SName) =>
                      //       SName.StateName.trim()
                      //         .toLowerCase()
                      //         .includes(keyName.trim().toLowerCase())
                      //     );
                      //
                      //     return <TableCell>{row[keyName]}</TableCell>;
                      //   })}
                      // </TableRow>;
                    })
                : null}
            </TableBody>
          </Table>
        </TableContainer>
        <TablePagination
          component="div"
          count={this.props.ReportCount}
          page={this.state.page}
          onChangePage={this.handleChangePage}
          rowsPerPage={this.state.rowsPerPage}
          onChangeRowsPerPage={this.handleChangeRowsPerPage}
        />
      </>
    );
  }
}

export default CustomColumn;
